Public Class ReportApp

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
        ImageBox_Label.Visible = True

    End Sub

    Private Sub CreateImage(Source As String)

        ImageBox.Image = DecodeImage(Source)
        ImageBox_Label.Visible = False

    End Sub


    Private Sub ReportApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' General Fields
        Dim Labels = {Title, Description, Ingredients, Procedure}
        For i = 0 To Labels.Length - 1
            Labels(i).Text = TheItem.SubItems(i).Text
        Next

        ' Image Parsing
        Dim ImgPath = TheItem.SubItems(4).Text
        If ImgPath = "" Then
            Clear_Image()
        Else
            CreateImage(ImgPath)
        End If

        ' Ingredients Parsing
        Dim Original = TheItem.SubItems(2).Text
        Dim Splitted = Original.Split(Delimiters)
        If Splitted.Length > 1 Then
            Ingredients.Text = String.Join(Environment.NewLine, Splitted.Select(Function(x) "• " & x.Trim()))
        Else
            Ingredients.Text = Original
        End If

    End Sub

End Class