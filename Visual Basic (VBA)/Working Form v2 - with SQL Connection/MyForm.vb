Public Class MyFormApp

	Private Sub Clear_Fields()
		Title.Text = ""
		Description.Text = ""
		Ingredients.Text = ""
		Procedure.Text = ""
	End Sub

	Private Sub Label_Message(ByRef msg As String)

		Status.Text = $"Data {msg}"

	End Sub

	Private Sub Update_List(
			ByRef Title As TextBox,
			ByRef Description As TextBox,
			ByRef Ingredients As TextBox,
			ByRef Procedure As TextBox,
			is_edit As Boolean
		)

		' Verify that list is not Empty
		If is_edit And TheList.SelectedItems.Count = 0 Then
			Return
		End If

		' Verify if the fields are empty
		If (Title.Text = "" Or Description.Text = "" Or Ingredients.Text = "" Or Procedure.Text = "") Then
			' Shows message box for each field, if empty
			Dim message = IIf(Title.Text = "",
							"Title",
							IIf(Description.Text = "",
							"Description",
							IIf(Ingredients.Text = "",
							"Ingredients",
							IIf(Procedure.Text = "",
							"Procedure",
							Nothing
			))))

			Dim new_message = "The field of " & message & " is empty"
			MessageBox.Show(new_message, "Empty Field detected", MessageBoxButtons.OK, MessageBoxIcon.Information)

			Return
		End If

		' Verify if the Recipe already exists
		For Each the_data As ListViewItem In TheList.Items

			Dim check_4_edit = True

			If (is_edit) Then
				check_4_edit = the_data.Index <> TheList.SelectedIndices(0)
			End If

			If the_data.Text = Title.Text And check_4_edit Then
				MessageBox.Show(
					"The Recipe of the similar Title already exists",
					"Duplicate detected",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
				)
				Return
			End If
		Next

		' Data Preparation
		Dim My_Data = New ListViewItem()
		Dim Manager = New QueryManager()

		If (is_edit) Then
			My_Data = TheList.SelectedItems(0)
			My_Data.SubItems.Clear()
		End If

		My_Data.Text = Title.Text
		My_Data.SubItems.Add(Description.Text)
		My_Data.SubItems.Add(Ingredients.Text)
		My_Data.SubItems.Add(Procedure.Text)

		' Update the List
		If Not is_edit Then
			TheList.Items.Add(My_Data)
			Manager.Insert(Title, Description, Ingredients, Procedure)
			Clear_Fields()

		Else
			Manager.Update(Title, Description, Ingredients, Procedure)
		End If

		Label_Message(IIf(is_edit, "Updated", "Added"))

	End Sub


	Private Sub AddRecipe_Click(sender As Object, e As EventArgs) Handles AddRecipe.Click

		Update_List(Title, Description, Ingredients, Procedure, False)

	End Sub

	Private Sub EditRecipe_Click(sender As Object, e As EventArgs) Handles EditRecipe.Click

		Update_List(Title, Description, Ingredients, Procedure, True)

	End Sub

	Private Sub DeleteRecipe_Click(sender As Object, e As EventArgs) Handles DeleteRecipe.Click

		' Verify that list is not Empty
		If TheList.SelectedItems.Count = 0 Then
			Return
		End If

		' Remove the selected data
		TheList.Items.RemoveAt(TheList.SelectedIndices(0))

		' Delete in the Database
		Dim Manager = New QueryManager()
		Manager.Delete(Title)

		Clear_Fields()
		Label_Message("Deleted")

	End Sub

	Private Sub TheList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TheList.SelectedIndexChanged

		' If the list is empty, then return
		If TheList.SelectedItems.Count = 0 Then
			Return
		End If

		' Load the data from the list, to the fields
		Dim m_Index = TheList.SelectedIndices(0)
		Dim my_data = TheList.SelectedItems(0)
		Title.Text = my_data.Text
		Description.Text = my_data.SubItems(1).Text
		Ingredients.Text = my_data.SubItems(2).Text
		Procedure.Text = my_data.SubItems(3).Text

	End Sub


	Private Sub MyFormApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load

		Dim Manager = New QueryManager()
		Manager.Reload(TheList)

	End Sub

	Private Class QueryManager

		Private ReadOnly The_Connection = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MyDB;Integrated Security=True"
		Private ReadOnly SQL_Connection = New SqlClient.SqlConnection(The_Connection)
		Private My_SQL_Command As SqlClient.SqlCommand

		Private Sub Execute_Query()
			SQL_Connection.Open()
			My_SQL_Command.ExecuteNonQuery()
			My_SQL_Command.Dispose()
			SQL_Connection.Close()
		End Sub

		' Reload List
		Public Sub Reload(ByRef TheList As ListView)
			' Format:
			' SELECT [Title], [Description], [Ingredients], [Procedure] FROM [dbo].[MyTable]

			TheList.Items.Clear()

			My_SQL_Command = New SqlClient.SqlCommand(
				"SELECT [Title], [Description], [Ingredients], [Procedure] FROM [dbo].[MyTable]",
				SQL_Connection
			)

			SQL_Connection.Open()
			Dim My_Reader = My_SQL_Command.ExecuteReader()
			While My_Reader.Read()
				Dim My_Item = New ListViewItem(My_Reader.GetString(0))
				My_Item.SubItems.Add(My_Reader.GetString(1))
				My_Item.SubItems.Add(My_Reader.GetString(2))
				My_Item.SubItems.Add(My_Reader.GetString(3))
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
			' INSERT INTO [dbo].[MyTable]
			' (
			'		[Title]
			'      ,[Description]
			'      ,[Ingredients]
			'      ,[Procedure]
			' )

			My_SQL_Command = New SqlClient.SqlCommand(
				String.Format(
					"INSERT INTO [dbo].[MyTable] ([Title], [Description], [Ingredients], [Procedure]) VALUES ('{0}', '{1}', '{2}', '{3}')",
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
			' UPDATE [dbo].[MyTable]
			' SET
			'		[Title] = '{0}'
			'      ,[Description] = '{1}'
			'      ,[Ingredients] = '{2}'
			'      ,[Procedure] = '{3}'
			' WHERE [Title] = '{0}'

			My_SQL_Command = New SqlClient.SqlCommand(
				String.Format(
					"UPDATE [dbo].[MyTable] SET [Title] = '{0}', [Description] = '{1}', [Ingredients] = '{2}', [Procedure] = '{3}' WHERE [Title] = '{0}'",
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
			' DELETE FROM [dbo].[MyTable]
			' WHERE [Title] = '{0}'

			My_SQL_Command = New SqlClient.SqlCommand(
				$"DELETE FROM [dbo].[MyTable] WHERE [Title] = '{Title.Text}'",
				SQL_Connection
			)
			Execute_Query()

		End Sub

	End Class

End Class