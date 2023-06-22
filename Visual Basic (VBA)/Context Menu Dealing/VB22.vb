' First, Create a Separate ContextMenuStrip Control.
' Then, Add the Items to the ContextMenuStrip.
' Then, Attach the ContextMenuStrip to the ListView Control.

Public Class VB22
    Private Sub MyList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles MyList.SelectedIndexChanged

        ' MyMenu is the ContextMenuStrip control.
        ' Sets the Items of the ContextMenuStrip as;
        ' - Copy
        ' - Cut
        ' - Delete
        ' Upon Clicking on the Items, just show a MessageBox.

        ' Modifying the Items of the ContextMenuStrip.
        MyMenu.Items.Clear()
        MyMenu.Items.Add("Copy")
        MyMenu.Items.Add("Cut")
        MyMenu.Items.Add("Delete")

        ' Adding the Click Event Handler to the Items.
        For i = 0 To MyMenu.Items.Count - 1
            Dim v = i
            AddHandler MyMenu.Items(i).Click, Sub() MessageBox.Show("You Clicked " & MyMenu.Items(v).Text)
        Next

    End Sub

End Class
