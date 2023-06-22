Public Class RecipeManager

	Dim ButtonName() As String = {"Add", "Update"}
	Dim Cache() As String = {"", "", "", ""}

	Private Class QueryManager

		Private Shared ReadOnly The_Table_Name = "dbo.RecipeDB"
		Private Shared ReadOnly The_Connection = Configuration.ConfigurationManager.ConnectionStrings("TheCS").ConnectionString
		Private Shared ReadOnly SQL_Connection = New SqlClient.SqlConnection(The_Connection)
		Private Shared My_SQL_Command As SqlClient.SqlCommand

		Private Shared Sub Execute_Query()
			SQL_Connection.Open()
			My_SQL_Command.ExecuteNonQuery()
			My_SQL_Command.Dispose()
			SQL_Connection.Close()
		End Sub

		Public Shared Sub Reload(ByRef TheList As ListView)

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
		Public Shared Sub Insert(
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
					"INSERT INTO {0} VALUES ('{1}', '{2}', '{3}', '{4}')",
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
		Public Shared Sub Update(
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
		Public Shared Sub Delete(ByRef Title As TextBox)

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

	Private Sub ClearFields()

		DeleteRecipe.Enabled = False
		With Title
			.Text = ""
			.Focus()
		End With
		Description.Text = ""
		Ingredients.Text = ""
		Procedure.Text = ""

	End Sub

	Private Sub LoadLocale()

		Dim My_IP = Net.Dns.GetHostEntry(Net.Dns.GetHostName()).AddressList(1).ToString()
		Dim TimeZ = TimeZone.CurrentTimeZone.StandardName

		Locale.Text = $"IP: {My_IP}   |   Timezone: {TimeZ}"

	End Sub

	Private Sub MyFormApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load

		Status.Text = ""
		QueryManager.Reload(TheList)
		ClearFields()
		LoadLocale()

	End Sub

	Private Sub AddRecipe_Click(sender As Object, e As EventArgs) Handles AddRecipe.Click

		Dim is_edit = AddRecipe.Text = ButtonName(1)

		' In Edit Mode, the Recipe must be selected
		If is_edit AndAlso TheList.SelectedItems.Count = 0 Then Return

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

		If is_edit Then
			QueryManager.Update(Title, Description, Ingredients, Procedure)
		Else
			QueryManager.Insert(Title, Description, Ingredients, Procedure)
		End If
		QueryManager.Reload(TheList)

		ClearFields()

		' Update the Button
		AddRecipe.Text = ButtonName(0)
		AddRecipe.Enabled = False

		Status.Text = IIf(is_edit, "Updated", "Added")

	End Sub

	Private Sub DeleteRecipe_Click(sender As Object, e As EventArgs) Handles DeleteRecipe.Click

		If TheList.SelectedItems.Count = 0 Then Return

		QueryManager.Delete(Title)
		QueryManager.Reload(TheList)

		ClearFields()
		Status.Text = "Deleted"

	End Sub

	Private Sub Clear_Click(sender As Object, e As EventArgs) Handles Clear.Click

		ClearFields()
		Status.Text = "Cleared"

	End Sub

	Private Sub TheList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TheList.SelectedIndexChanged

		If TheList.SelectedItems.Count = 0 Then
			DeleteRecipe.Enabled = False
			Return
		End If

		' LastSelected = TheList.SelectedIndices(0)

		With TheList.SelectedItems(0)
			Title.Text = .Text
			Description.Text = .SubItems(1).Text
			Ingredients.Text = .SubItems(2).Text
			Procedure.Text = .SubItems(3).Text
		End With

		For i = 0 To 3
			Cache(i) = TheList.SelectedItems(0).SubItems(i).Text
		Next

		AddRecipe.Text = ButtonName(1)
		AddRecipe.Enabled = False
		DeleteRecipe.Enabled = True

	End Sub

	Private Sub AddButtonUpdate(sender As Object, e As EventArgs) Handles Title.TextChanged, Procedure.TextChanged, Ingredients.TextChanged, Description.TextChanged

		' Flow
		' ----
		' If:		 Title is empty or Title is not equal to Cache(0)	-> "Add"	Enabled
		' ElseIf:	 Any of the fields are updated						-> "Update"	Enabled
		' Else:															-> "Add"	Disabled

		If (Title.Text = "" Or Title.Text <> Cache(0)) Then
			AddRecipe.Text = ButtonName(0)
			AddRecipe.Enabled = True
		ElseIf (
			Description.Text <> Cache(1) Or
			Ingredients.Text <> Cache(2) Or
			Procedure.Text <> Cache(3)
		) Then
			AddRecipe.Text = ButtonName(1)
			AddRecipe.Enabled = True
		Else
			AddRecipe.Text = ButtonName(0)
			AddRecipe.Enabled = False
		End If

	End Sub

End Class