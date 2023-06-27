Module Utility

	Public ReadOnly Delimiters As Char() = {",", Environment.NewLine}

	Public Function EncodeImage(ByRef image As Image) As String

		If image IsNot Nothing Then
			Using stream = New IO.MemoryStream()
				image.Save(stream, image.RawFormat)
				Return Convert.ToBase64String(stream.ToArray())
			End Using
		Else
			Return Nothing
		End If

	End Function

	Public Function DecodeImage(ByRef imageBase64 As String) As Image

		Dim imageBytes = Convert.FromBase64String(imageBase64)
		Return Image.FromStream(New IO.MemoryStream(imageBytes))

	End Function

End Module