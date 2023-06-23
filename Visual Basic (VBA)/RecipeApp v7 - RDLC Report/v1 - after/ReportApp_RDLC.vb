Public Class ReportApp_RDLC

	Private Sub ReportRDLC_Load(sender As Object, e As EventArgs) Handles MyBase.Load

		RecipeDBTableAdapter.Fill(MyRecipeDataSet.RecipeDB)

		' Formatting the Ingredients Column
		For Each MyRow As DataRow In MyRecipeDataSet.RecipeDB.Rows
			Dim Original = MyRow.Field(Of String)(2)
			Dim Splitted = Original.Split(",")

			If Splitted.Length > 1 Then
				MyRow.SetField(2, String.Join(Environment.NewLine, Splitted.Select(Function(x) "• " & x.Trim())))
			End If
		Next

		MyReportView.RefreshReport()

	End Sub
End Class