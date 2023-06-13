Imports System.Reflection

Public Class MyFormApp

	Public Sub Update_List(
			ByRef Title As TextBox,
			ByRef Description As TextBox,
			ByRef Ingredients As TextBox,
			ByRef Procedure As TextBox,
			ByVal is_edit As Boolean
		)

		' Verify that list is not Empty
		If TheList.SelectedItems.Count = 0 And is_edit Then
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
		Dim my_data As New ListViewItem()

		If (is_edit) Then
			my_data = TheList.SelectedItems(0)
			my_data.SubItems.Clear()
		End If

		my_data.Text = Title.Text
		my_data.SubItems.Add(Description.Text)
		my_data.SubItems.Add(Ingredients.Text)
		my_data.SubItems.Add(Procedure.Text)

		' Update the List, if not edit
		If Not is_edit Then
			TheList.Items.Add(my_data)

			' Clear the fields
			Title.Text = ""
			Description.Text = ""
			Ingredients.Text = ""
			Procedure.Text = ""
		End If

		' Display Message
		Dim m_status = ""
		If (is_edit) Then
			m_status = "Edited"
		Else
			m_status = "Added"
		End If
		Status.Text = $"Data {m_status} ✅"

	End Sub

	Private Sub AddRecipe_Click(sender As Object, e As EventArgs) Handles AddRecipe.Click

		Update_List(Title, Description, Ingredients, Procedure, False)

	End Sub

	Private Sub DeleteRecipe_Click(sender As Object, e As EventArgs) Handles DeleteRecipe.Click

		' Verify that list is not Empty
		If TheList.SelectedItems.Count = 0 Then
			Return
		End If

		' Remove the selected data
		TheList.Items.RemoveAt(TheList.SelectedIndices(0))

		' Clear the fields
		Title.Text = ""
		Description.Text = ""
		Ingredients.Text = ""
		Procedure.Text = ""

		' Status Update
		Status.Text = "Deleted ✅"

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

	Private Sub EditRecipe_Click(sender As Object, e As EventArgs) Handles EditRecipe.Click

		Update_List(Title, Description, Ingredients, Procedure, True)

	End Sub

End Class
