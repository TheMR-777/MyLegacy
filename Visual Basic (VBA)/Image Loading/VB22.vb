Public Class VB22
    Private Sub DisplayButton_Click(sender As Object, e As EventArgs) Handles DisplayButton.Click

        Dim MyImage = Bitmap.FromFile("C:\Users\Muhammad Ammar Khan\Downloads\my_image.jpg")
        ImageBox.Image = MyImage

    End Sub
End Class
