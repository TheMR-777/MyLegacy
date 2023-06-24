Public Class RecipeManager

	ReadOnly ImageLabel = "Click to Add an Image"
	Dim ButtonName() As String = {"Create", "Update"}
	Dim Cache() As String = {"", "", "", ""}

	' Utility Functions
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

		Public Shared Sub ReloadList(ByRef TheList As ListView)

			Dim Format = $"SELECT [Title], [Description], [Ingredients], [Procedure], [Image] FROM {The_Table_Name}"

			TheList.Items.Clear()

			My_SQL_Command = New SqlClient.SqlCommand(
				Format,
				SQL_Connection
			)

			SQL_Connection.Open()
			Dim MyReader = My_SQL_Command.ExecuteReader()
			While MyReader.Read()

				Dim TheItem = New ListViewItem(MyReader.GetString(0))
				With TheItem.SubItems
					.Add(MyReader.GetString(1))
					.Add(MyReader.GetString(2))
					.Add(MyReader.GetString(3))
					.Add(MyReader.GetString(4))
				End With
				TheList.Items.Add(TheItem)

			End While
			SQL_Connection.Close()

		End Sub

		' Insertion
		Public Shared Sub Insert(
				ByRef Title As TextBox,
				ByRef Description As TextBox,
				ByRef Ingredients As TextBox,
				ByRef Procedure As TextBox,
				ByRef ImageBox As PictureBox
			)

			' Format:
			' INSERT INTO {My_Table_Name}
			' (
			'		[Title]
			'      ,[Description]
			'      ,[Ingredients]
			'      ,[Procedure]
			'	   ,[Image]
			' )

			My_SQL_Command = New SqlClient.SqlCommand(
				String.Format(
					"INSERT INTO {0} VALUES ('{1}', '{2}', '{3}', '{4}', '{5}')",
					The_Table_Name,
					Title.Text,
					Description.Text,
					Ingredients.Text,
					Procedure.Text,
					EncodeImage(ImageBox.Image)
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
				ByRef Procedure As TextBox,
				ByRef ImageBox As PictureBox
			)

			' Format:
			' UPDATE {My_Table_Name}
			' SET
			'		[Title] = '{0}'
			'      ,[Description] = '{}'
			'      ,[Ingredients] = '{}'
			'      ,[Procedure] = '{}'
			'      ,[Image] = '{}'
			' WHERE [Title] = '{0}'

			My_SQL_Command = New SqlClient.SqlCommand(
				String.Format(
					"UPDATE {0} SET [Title] = '{1}', [Description] = '{2}', [Ingredients] = '{3}', [Procedure] = '{4}', [Image] = '{5}' WHERE [Title] = '{1}'",
					The_Table_Name,
					Title.Text,
					Description.Text,
					Ingredients.Text,
					Procedure.Text,
					EncodeImage(ImageBox.Image)
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

	Private Sub Clear_Image()

		ImageDisplay.Image = Nothing
		ImageDisplay_Label.Text = ImageLabel

	End Sub

	Private Sub CreateImage(ByRef Source As String, is_encoded As Boolean)

		ImageDisplay.Image = If(is_encoded, DecodeImage(Source), Image.FromFile(Source))
		ImageDisplay_Label.Text = Nothing

	End Sub

	Private Sub ClearFields()

		DeleteRecipe.Enabled = False
		With Title
			.Text = ""
			.Focus()
		End With
		Description.Text = ""
		Ingredients.Text = ""
		Procedure.Text = ""
		Clear_Image()

	End Sub

	Private Sub Load_Locale()

		Dim My_IP As String
		Dim TimeZ As String

		Try

			Dim MyClient = New Net.WebClient()
			Dim DataJSON = MyClient.DownloadString("http://ip-api.com/json")
			Dim Raw_Data = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(DataJSON)

			My_IP = Raw_Data("query")
			TimeZ = Raw_Data("timezone")

			' Dim Raw_Data = DataJSON.Substring(1, DataJSON.Length - 2).Split(New Char() {":", ","}).ToList()
			' My_IP = Raw_Data(27).Substring(1, Raw_Data(27).Length - 2)
			' TimeZ = Raw_Data(19).Substring(1, Raw_Data(19).Length - 2)

		Catch

			' My_IP = Net.Dns.GetHostEntry(Net.Dns.GetHostName()).AddressList(1).ToString()
			My_IP = "No Internet"
			TimeZ = TimeZoneInfo.Local.DisplayName

		End Try

		Dim My_Locale = $"IP: {My_IP}   |   Timezone: {TimeZ}"

		Locale.Text = My_Locale

	End Sub

	Private Sub UpdateImage(is_double As Boolean)

		If (is_double = IsNothing(ImageDisplay.Image)) Then Return

		Dim DialogBox = New OpenFileDialog()
		If (DialogBox.ShowDialog() = DialogResult.OK) Then
			CreateImage(DialogBox.FileName, is_encoded:=False)
			AddRecipe.Enabled = True
		End If

	End Sub

	' Events
	Private Sub MyFormApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load

		Status.Text = ""
		AddRecipe.Text = ButtonName(0)
		QueryManager.ReloadList(TheList)
		ClearFields()
		Load_Locale()

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
			QueryManager.Update(Title, Description, Ingredients, Procedure, ImageDisplay)
		Else
			QueryManager.Insert(Title, Description, Ingredients, Procedure, ImageDisplay)
		End If
		QueryManager.ReloadList(TheList)

		ClearFields()

		' Update the Button
		AddRecipe.Text = ButtonName(0)
		AddRecipe.Enabled = False

		Status.Text = IIf(is_edit, "Updated", "Created")

	End Sub

	Private Sub DeleteRecipe_Click(sender As Object, e As EventArgs) Handles DeleteRecipe.Click

		If TheList.SelectedItems.Count = 0 Then Return

		QueryManager.Delete(Title)
		QueryManager.ReloadList(TheList)

		ClearFields()
		Status.Text = "Deleted"

	End Sub

	Private Sub Clear_Click(sender As Object, e As EventArgs) Handles Clear.Click

		ClearFields()
		Status.Text = "Cleared"

	End Sub

	Private Sub TheList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TheList.SelectedIndexChanged

		If TheList.SelectedItems.Count = 0 Then
			RemoveHandler ListMenu.Items(1).Click, AddressOf DeleteRecipe_Click
			DeleteRecipe.Enabled = False
			Return
		End If

		' Attaching the Events with Context Menu
		AddHandler ListMenu.Items(1).Click, AddressOf DeleteRecipe_Click
		AddHandler ListMenu.Items(0).Click,
			Sub()
				Dim ReportForm = New ReportApp(TheList.SelectedItems(0))
				ReportForm.Show()
			End Sub

		' Removing the Event Handlers, so that the Add Button is not updated
		Dim Fields = {Title, Description, Ingredients, Procedure}
		For Each Box In Fields
			RemoveHandler Box.TextChanged, AddressOf AddButtonUpdate
		Next

		' Updating the Fields
		With TheList.SelectedItems(0)
			Title.Text = .Text
			Description.Text = .SubItems(1).Text
			Ingredients.Text = .SubItems(2).Text
			Procedure.Text = .SubItems(3).Text

			Dim Image64 = .SubItems(4).Text
			If Image64 = "" Then
				Clear_Image()
			Else
				CreateImage(Image64, True)
			End If
		End With

		' Adding the Event Handlers back
		For Each Box In Fields
			AddHandler Box.TextChanged, AddressOf AddButtonUpdate
		Next

		' Updating the Cache, for the Update Button
		For i = 0 To Cache.Length - 1
			Cache(i) = TheList.SelectedItems(0).SubItems(i).Text
		Next

		Status.Text = ""
		AddRecipe.Text = ButtonName(1)
		AddRecipe.Enabled = False
		DeleteRecipe.Enabled = True

	End Sub

	Private Sub AddButtonUpdate(sender As Object, e As EventArgs) Handles Title.TextChanged, Procedure.TextChanged, Ingredients.TextChanged, Description.TextChanged

		' Flow
		' ----
		' If:		 Title is empty or Title is not equal to its Cache	-> "Add"	Enabled
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

		Status.Text = ""

	End Sub

	Private Sub ImageLocation_Click(sender As Object, e As EventArgs) Handles ImageDisplay.Click, ImageDisplay_Label.Click

		UpdateImage(False)

	End Sub

	Private Sub ImageLocation_DoubleClick(sender As Object, e As EventArgs) Handles ImageDisplay.DoubleClick

		UpdateImage(True)

	End Sub

	Private Sub ViewReportRDLC_Click(sender As Object, e As EventArgs) Handles ViewReportRDLC.Click

		' Opens the Report Viewer
		With New ReportApp_RDLC()
			.ShowDialog()
		End With

	End Sub

End Class