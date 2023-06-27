
Public Class Registration

    ' Liability
    Public TheLogin As Login

    Public Function IsValidEmail(email As String) As Boolean

        Return New Text.RegularExpressions.Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").Match(email).Success

    End Function

    ' Events
    Private Sub Registration_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' User Fields:
        ' - F_Name
        ' - L_Name
        ' - Email
        ' - Password

    End Sub

    Private Sub RegisterButton_Click(sender As Object, e As EventArgs) Handles RegisterButton.Click

        For Each Field In {FirstName, LastName, Email, Password}
            If Field.Text = "" Then
                MessageBox.Show(
                    $"The Field of {Field.Name} is empty",
                    "Empty Field detected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                )
                Field.Focus()
                Return
            End If
        Next

        If Not IsValidEmail(Email.Text) Then
            MessageBox.Show(
                "The Email is not valid",
                "Invalid Email detected",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )
            Email.Focus()
            Return
        End If

        If Not UserQuery.Register(FirstName, LastName, Email, Password) Then
            MessageBox.Show(
                "The Email is already registered",
                "Duplicate detected",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )
            Return
        End If

        MessageBox.Show(
            "The User is registered",
            "Registration completed",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        )

        'With New RecipeApp(Email.Text)
        '    .TheLogin = Nothing
        '    .Register = Me
        '    .Show()
        'End With

        With TheLogin
            .PreLoad(Email.Text, Password.Text)
            .Show()
        End With
        Close()

    End Sub

    ' Closers
    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click

        Close()

    End Sub

    Private Sub GoLogin_Click(sender As Object, e As EventArgs) Handles GoLogin.Click

        Close()

    End Sub

    Private Sub Registration_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        TheLogin?.Show()

    End Sub
End Class