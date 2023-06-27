Public Class RecipeApp

	Dim ButtonName() As String = {"Create", "Update"}
	Dim Cache() As String = {"", "", "", ""}
	Dim DidSignOut = False
	Public Shared TheLogin As Login

	Public Sub New(ByRef User As String)
		InitializeComponent()
		Welcome.Text = $"Welcome, {User}"
	End Sub

	' Utility Functions
	Private Class RecipeQuery

		Private Shared ReadOnly TableRecipeDB = "dbo.RecipeDB"
		Private Shared ReadOnly ConnectionStr = Configuration.ConfigurationManager.ConnectionStrings("TheCS").ConnectionString
		Private Shared ReadOnly DB_Connection = New SqlClient.SqlConnection(ConnectionStr)
		Private Shared DatabaseQuery As SqlClient.SqlCommand

		Private Shared Sub Execute_Query()
			DB_Connection.Open()
			DatabaseQuery.ExecuteNonQuery()
			DatabaseQuery.Dispose()
			DB_Connection.Close()
		End Sub

		Public Shared Sub ReloadList(ByRef RecipeList As ListView)

			Dim Format = $"SELECT 
							  [Title]
							, [Description]
							, [Ingredients]
							, [Procedure]
							, [Image] 
						 FROM {TableRecipeDB}"

			RecipeList.Items.Clear()
			DB_Connection.Open()
			Dim RowIterator = New SqlClient.SqlCommand(Format, DB_Connection).ExecuteReader()

			While RowIterator.Read()
				Dim TheItem = New ListViewItem(RowIterator.GetString(0))
				With TheItem.SubItems
					.Add(RowIterator.GetString(1))
					.Add(RowIterator.GetString(2))
					.Add(RowIterator.GetString(3))
					.Add(RowIterator.GetString(4))
				End With
				RecipeList.Items.Add(TheItem)
			End While

			DB_Connection.Close()

		End Sub

		' Insertion
		Public Shared Sub Insert(
				ByRef Title As TextBox,
				ByRef Description As TextBox,
				ByRef Ingredients As TextBox,
				ByRef Procedure As TextBox,
				ByRef ImageBox As PictureBox
			)

			Dim Format = $"INSERT INTO {TableRecipeDB} VALUES 
            (
                @Title, 
                @Description, 
                @Ingredients, 
                @Procedure, 
                @Image
            )"

			DatabaseQuery = New SqlClient.SqlCommand(Format, DB_Connection)
			DatabaseQuery.Parameters.AddWithValue("@Title", Title.Text)
			DatabaseQuery.Parameters.AddWithValue("@Description", Description.Text)
			DatabaseQuery.Parameters.AddWithValue("@Ingredients", Ingredients.Text)
			DatabaseQuery.Parameters.AddWithValue("@Procedure", Procedure.Text)
			DatabaseQuery.Parameters.AddWithValue("@Image", EncodeImage(ImageBox.Image))
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

			Dim Format = $"UPDATE {TableRecipeDB} 
            SET 
                [Title] = @Title, 
                [Description] = @Description, 
                [Ingredients] = @Ingredients, 
                [Procedure] = @Procedure, 
                [Image] = @Image 
            WHERE [Title] = @Title"

			DatabaseQuery = New SqlClient.SqlCommand(Format, DB_Connection)
			DatabaseQuery.Parameters.AddWithValue("@Title", Title.Text)
			DatabaseQuery.Parameters.AddWithValue("@Description", Description.Text)
			DatabaseQuery.Parameters.AddWithValue("@Ingredients", Ingredients.Text)
			DatabaseQuery.Parameters.AddWithValue("@Procedure", Procedure.Text)
			DatabaseQuery.Parameters.AddWithValue("@Image", EncodeImage(ImageBox.Image))
			Execute_Query()

		End Sub

		' Deletion
		Public Shared Sub Delete(ByRef Title As TextBox)

			DatabaseQuery = New SqlClient.SqlCommand(
				$"DELETE FROM {TableRecipeDB} WHERE [Title] = '{Title.Text}'",
				DB_Connection
			)
			Execute_Query()

		End Sub

	End Class

	Private Sub Clear_Image()

		ImageDisplay.Image = Nothing
		ImageDisplay_Label.Visible = True

	End Sub

	Private Sub CreateImage(ByRef Source As String, is_encoded As Boolean)

		ImageDisplay.Image = If(is_encoded, DecodeImage(Source), Image.FromFile(Source))
		ImageDisplay_Label.Visible = False

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

	Private Sub Load_Online()

		Dim WeatherAPI = "285b4f0fedd6a1ae3793dff944b724d7"
		Dim WeatherURL = "https://api.openweathermap.org/data/2.5/weather"
		Dim Locale_URL = "http://ip-api.com/json"
		Dim Client = New Net.WebClient()
		Dim Locale As Dictionary(Of String, String)
		Dim OpenWeather As Newtonsoft.Json.Linq.JObject

		Try
			Dim Buffer As String

			Buffer = Client.DownloadString(Locale_URL)
			Locale = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(Buffer)

			Buffer = Client.DownloadString(
				$"{WeatherURL}
				?lat={Locale("lat")}
				&lon={Locale("lon")}
				&appid={WeatherAPI}"
			)
			OpenWeather = Newtonsoft.Json.Linq.JObject.Parse(Buffer)
		Catch ex As Net.WebException
			MessageBox.Show($"Network Error: {ex.Message}")
			Return
		Catch ex As Newtonsoft.Json.JsonException
			MessageBox.Show($"Error parsing the Data: {ex.Message}")
			Return
		Catch ex As Exception
			MessageBox.Show($"An Error occurred: {ex.Message}")
			Return
		End Try

		Dim IPAddress = If(Locale.ContainsKey("query"), Locale("query"), "No Internet")
		Dim Time_Zone = If(Locale.ContainsKey("timezone"), Locale("timezone"), TimeZoneInfo.Local.DisplayName)
		Me.Locale.Text = $"IP: {IPAddress}   |   Timezone: {Time_Zone}"

		Dim Temperature = Math.Round((CDbl(OpenWeather("main")("temp")) - 273.15), 1)
		Dim The_Weather = OpenWeather("weather")(0)("main").ToString()
		Dim Description = OpenWeather("weather")(0)("description").ToString()
		Dim WeatherIcon = OpenWeather("weather")(0)("icon").ToString()

		Weather.Text = The_Weather
		WeatherTemperature.Text = Temperature.ToString() & "°C"
		WeatherDescription.Text = Description
		WeatherLogo.ImageLocation = $"http://openweathermap.org/img/w/{WeatherIcon}.png"

	End Sub


	Private Sub UpdateImage(IsDoubleTap As Boolean)

		If IsDoubleTap = IsNothing(ImageDisplay.Image) Then Return

		Dim DialogBox = New OpenFileDialog()
		If (DialogBox.ShowDialog() = DialogResult.OK) Then
			CreateImage(DialogBox.FileName, is_encoded:=False)
			AddRecipe.Enabled = True
		End If

	End Sub

	' Events
	Private Sub RecipeApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load

		Status.Text = ""
		AddRecipe.Text = ButtonName(0)
		RecipeQuery.ReloadList(TheRecipeList)
		ClearFields()
		Load_Online()

	End Sub

	Private Sub AddRecipe_Click(sender As Object, e As EventArgs) Handles AddRecipe.Click

		Dim is_edit = AddRecipe.Text = ButtonName(1)

		' In Edit Mode, the Recipe must be selected
		If is_edit AndAlso TheRecipeList.SelectedItems.Count = 0 Then Return

		' Verify if the fields are not empty
		For Each Field In {Title, Description, Ingredients, Procedure}
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

		' Verify if the Recipe already exists
		For Each Recipe In TheRecipeList.Items
			If is_edit AndAlso Recipe.Index = TheRecipeList.SelectedIndices(0) Then
				Continue For
			End If

			If Recipe.Text = Title.Text Then
				MessageBox.Show(
					"The Recipe of the similar Title already exists",
					"Duplicate detected",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
				)
				Title.Focus()
				Return
			End If
		Next

		If is_edit Then
			RecipeQuery.Update(Title, Description, Ingredients, Procedure, ImageDisplay)
		Else
			RecipeQuery.Insert(Title, Description, Ingredients, Procedure, ImageDisplay)
		End If
		RecipeQuery.ReloadList(TheRecipeList)

		ClearFields()

		' Update the Button
		AddRecipe.Text = ButtonName(0)
		AddRecipe.Enabled = False

		Status.Text = If(is_edit, "Updated", "Created")

	End Sub

	Private Sub DeleteRecipe_Click(sender As Object, e As EventArgs) Handles DeleteRecipe.Click

		If TheRecipeList.SelectedItems.Count = 0 Then Return

		RecipeQuery.Delete(Title)
		RecipeQuery.ReloadList(TheRecipeList)

		ClearFields()
		Status.Text = "Deleted"

	End Sub

	Private Sub Clear_Click(sender As Object, e As EventArgs) Handles Clear.Click

		ClearFields()
		Status.Text = "Cleared"
		TheRecipeList.SelectedItems.Clear()

	End Sub

	Private Sub TheList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TheRecipeList.SelectedIndexChanged

		If TheRecipeList.SelectedItems.Count = 0 Then
			RemoveHandler ListMenu.Items(1).Click, AddressOf DeleteRecipe_Click
			DeleteRecipe.Enabled = False
			Return
		End If

		' Attaching the Events with Context Menu
		AddHandler ListMenu.Items(1).Click, AddressOf DeleteRecipe_Click
		Dim handler =
			Sub()
				If Not ReportApp.IsLoaded Then
					Dim ReportForm = New ReportApp(TheRecipeList.SelectedItems(0))
					ReportApp.IsLoaded = True
					ReportForm.Show()
				End If
			End Sub

		RemoveHandler ListMenu.Items(0).Click, handler
		AddHandler ListMenu.Items(0).Click, handler


		' Removing the Event Handlers, so that the Add Button is not updated
		Dim Fields = {Title, Description, Ingredients, Procedure}
		For Each Box In Fields
			RemoveHandler Box.TextChanged, AddressOf AddButtonUpdate
		Next

		' Updating the Fields
		With TheRecipeList.SelectedItems(0)
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
			Cache(i) = TheRecipeList.SelectedItems(0).SubItems(i).Text
		Next

		Status.Text = ""
		AddRecipe.Text = ButtonName(1)
		AddRecipe.Enabled = False
		DeleteRecipe.Enabled = True

	End Sub

	Private Sub AddButtonUpdate(sender As Object, e As EventArgs) Handles Title.TextChanged, Procedure.TextChanged, Ingredients.TextChanged, Description.TextChanged

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

		With New ReportApp_RDLC()
			.ShowDialog()
		End With

	End Sub

	Private Sub RecipeApp_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

		If Not DidSignOut Then
			TheLogin.Close()
		End If

	End Sub

	Private Sub SignOutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SignOutToolStripMenuItem.Click

		DidSignOut = True
		TheLogin.Show()
		Close()

	End Sub

End Class