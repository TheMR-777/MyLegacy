Public Class MyForm
	Private Sub MyForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		AddHandler Txt_Mrks.TextChanged, AddressOf Txt_Mrks_TextChanged
	End Sub

	Function Calculate_CGPA(marks As String) As String
		If (String.IsNullOrEmpty(marks)) Then
			Return "CGPA"
		End If

		Select Case CSng(marks)
			Case Is > 80
				Return 4
			Case Is > 60
				Return 3
			Case Is > 40
				Return 2
			Case Is > 20
				Return 1
			Case Else
				Return 0
		End Select
	End Function

	Private Sub Txt_Mrks_TextChanged(sender As Object, e As EventArgs) Handles Txt_Mrks.TextChanged
		Txt_CGPA.Text = Calculate_CGPA(Txt_Mrks.Text)
	End Sub
End Class
