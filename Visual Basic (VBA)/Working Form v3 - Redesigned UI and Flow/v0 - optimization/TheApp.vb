Public Class RecipeManager

	Dim TitleCache = ""
	Dim OtherCache = ""

	Private Sub Clear_Fields()
		Title.Text = ""
		Description.Text = ""
		Ingredients.Text = ""
		Procedure.Text = ""
	End Sub

	Private Sub Label_Message(ByRef msg As String)

		Status.Text = msg

	End Sub

	Private Sub Update_List(
			ByRef Title As TextBox,
			ByRef Description As TextBox,
			ByRef Ingredients As TextBox,
			ByRef Procedure As TextBox,
			is_edit As Boolean
		)

		' In Edit Mode, the Recipe must be selected
		If is_edit And TheList.SelectedItems.Count = 0 Then
			Return
		End If

		' Verify if the fields are not empty
		For Each Field In {Title, Description, Ingredients, Procedure}
			If Field.Text = "" Then
				MessageBox.Show(
					$"The Field of {Field.Name} is empty",
					"Empty Field detected",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
				)
				Return
			End If
		Next

		' Verify if the Recipe already exists
		For Each Recipe In TheList.Items
			If is_edit AndAlso Recipe.Index = TheList.SelectedIndices(0) Then
				Continue For
			End If

			If Recipe.Text = Title.Text Then
				MessageBox.Show(
					"The Recipe of the similar Title already exists",
					"Duplicate detected",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
				)
				Return
			End If
		Next


		With New QueryManager()
			If is_edit Then
				.Update(Title, Description, Ingredients, Procedure)
			Else
				.Insert(Title, Description, Ingredients, Procedure)
				Clear_Fields()
			End If
			.Reload(TheList)
		End With

		Label_Message(IIf(is_edit, "Updated", "Added"))

	End Sub


	Private Sub AddRecipe_Click(sender As Object, e As EventArgs) Handles AddRecipe.Click

		Update_List(Title, Description, Ingredients, Procedure, False)

	End Sub

	Private Sub EditRecipe_Click(sender As Object, e As EventArgs)

		Update_List(Title, Description, Ingredients, Procedure, True)

	End Sub

	Private Sub DeleteRecipe_Click(sender As Object, e As EventArgs) Handles DeleteRecipe.Click

		If TheList.SelectedItems.Count = 0 Then
			Return
		End If

		With New QueryManager()
			.Delete(Title)
			.Reload(TheList)
		End With

		Clear_Fields()
		Label_Message("Deleted")

	End Sub

	Private Sub TheList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TheList.SelectedIndexChanged

		If TheList.SelectedItems.Count = 0 Then
			Return
		End If

		With TheList.SelectedItems(0)
			Title.Text = .Text
			Description.Text = .SubItems(1).Text
			Ingredients.Text = .SubItems(2).Text
			Procedure.Text = .SubItems(3).Text
		End With

	End Sub


	Private Sub MyFormApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load

		Status.Text = ""
		With New QueryManager()
			.Reload(TheList)
		End With

	End Sub

	Private Class QueryManager

		Private ReadOnly The_Table_Name = "[dbo].[RecipeDB]"

		Private ReadOnly The_Connection = Configuration.ConfigurationManager.ConnectionStrings("TheCS").ConnectionString
		Private ReadOnly SQL_Connection = New SqlClient.SqlConnection(The_Connection)
		Private My_SQL_Command As SqlClient.SqlCommand

		Private Sub Execute_Query()
			SQL_Connection.Open()
			My_SQL_Command.ExecuteNonQuery()
			My_SQL_Command.Dispose()
			SQL_Connection.Close()
		End Sub

		Public Sub Reload(ByRef TheList As ListView)

			Dim Format = $"SELECT [Title], [Description], [Ingredients], [Procedure] FROM {The_Table_Name}"

			TheList.Items.Clear()

			My_SQL_Command = New SqlClient.SqlCommand(
				Format,
				SQL_Connection
			)

			SQL_Connection.Open()
			Dim My_Reader = My_SQL_Command.ExecuteReader()
			While My_Reader.Read()

				Dim My_Item = New ListViewItem(My_Reader.GetString(0))
				With My_Item.SubItems
					.Add(My_Reader.GetString(1))
					.Add(My_Reader.GetString(2))
					.Add(My_Reader.GetString(3))
				End With
				TheList.Items.Add(My_Item)

			End While
			SQL_Connection.Close()

		End Sub

		' Insertion
		Public Sub Insert(
				ByRef Title As TextBox,
				ByRef Description As TextBox,
				ByRef Ingredients As TextBox,
				ByRef Procedure As TextBox
			)

			' Format:
			' INSERT INTO {My_Table_Name}
			' (
			'		[Title]
			'      ,[Description]
			'      ,[Ingredients]
			'      ,[Procedure]
			' )

			My_SQL_Command = New SqlClient.SqlCommand(
				String.Format(
					"INSERT INTO {0} ([Title], [Description], [Ingredients], [Procedure]) VALUES ('{1}', '{2}', '{3}', '{4}')",
					The_Table_Name,
					Title.Text,
					Description.Text,
					Ingredients.Text,
					Procedure.Text
				),
				SQL_Connection
			)
			Execute_Query()

		End Sub

		' Updation
		Public Sub Update(
				ByRef Title As TextBox,
				ByRef Description As TextBox,
				ByRef Ingredients As TextBox,
				ByRef Procedure As TextBox
			)

			' Format:
			' UPDATE {My_Table_Name}
			' SET
			'		[Title] = '{0}'
			'      ,[Description] = '{1}'
			'      ,[Ingredients] = '{2}'
			'      ,[Procedure] = '{3}'
			' WHERE [Title] = '{0}'

			My_SQL_Command = New SqlClient.SqlCommand(
				String.Format(
					"UPDATE {0} SET [Title] = '{1}', [Description] = '{2}', [Ingredients] = '{3}', [Procedure] = '{4}' WHERE [Title] = '{1}'",
					The_Table_Name,
					Title.Text,
					Description.Text,
					Ingredients.Text,
					Procedure.Text
				),
				SQL_Connection
			)
			Execute_Query()

		End Sub

		' Deletion
		Public Sub Delete(ByRef Title As TextBox)

			' Format:
			' DELETE FROM {My_Table_Name}
			' WHERE [Title] = '{0}'

			My_SQL_Command = New SqlClient.SqlCommand(
				$"DELETE FROM {The_Table_Name} WHERE [Title] = '{Title.Text}'",
				SQL_Connection
			)
			Execute_Query()

		End Sub

	End Class

	Private Sub Description_Enter(sender As Object, e As EventArgs) Handles Description.Enter

	End Sub

	Private Sub Description_TextChanged(sender As Object, e As EventArgs) Handles Description.TextChanged

	End Sub

	Private Sub Title_Enter(sender As Object, e As EventArgs) Handles Title.Enter



	End Sub
End Class