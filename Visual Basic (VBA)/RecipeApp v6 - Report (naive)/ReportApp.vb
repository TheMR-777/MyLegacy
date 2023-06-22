Public Class ReportApp

    Private ReadOnly ImageLabel As String = "No Image Available"
    Private TheItem As ListViewItem

    ' Constructor
    Public Sub New(ByRef Item As ListViewItem)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        TheItem = Item

    End Sub

    Private Sub Clear_Image()

        ImageBox.Image = Nothing
        ImageBox.ImageLocation = Nothing
        ImageBox_Label.Text = ImageLabel

    End Sub

    Private Sub CreateImage(Source As String)

        ImageBox.Image = Image.FromFile(Source)
        ImageBox.ImageLocation = Source
        ImageBox_Label.Text = ""
        ImageBox_Label.Visible = False

    End Sub


    Private Sub ReportApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim Labels = New Label() {Title, Description, Ingredients, Procedure}
        For i = 0 To Labels.Length - 1
            Labels(i).Text = TheItem.SubItems(i).Text
        Next

        Dim ImgPath = TheItem.SubItems(4).Text
        If ImgPath = "" Then
            Clear_Image()
        Else
            CreateImage(ImgPath)
        End If

    End Sub

End Class