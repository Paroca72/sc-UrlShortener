
Partial Class _Default
    Inherits SCFramework.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Check if must be a redirect
            If Me.ClientQueryString <> String.Empty Then
                ' Redirect
                Me.MakeRedirect()
            End If

            ' Set the controls on the page
            Me.SetControls()
        End If
    End Sub

#Region " COMMON "

    ' Open a message on screen
    Public Sub AddAlertMessage(Message As String)
        Message = SCFramework.HTML.FixJavaScriptString(Message)
        Dim Script As String = String.Format("<script type=""text/javascript"">alert(""{0}"");</script>", Message)
        ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "Warning", Script, False)
    End Sub

    ' Set the controls
    Private Sub SetControls()
        ' Buttons
        Me.BtnAction.Attributes("data-loading-text") = "In progress ..."
        Me.BtnAction.OnClientClick = "$(this).button('loading');"
    End Sub

#End Region

#Region " REDIRECT "

    ' Make a redirect to long URL
    Private Sub MakeRedirect()
        ' Extract the ID of the row in database
        Dim ID As Long = Global.Encoder.DecodeStringToNumber(Me.ClientQueryString)

        ' Get the URL
        Dim URL As String = Global.URLs.GetLongURL(ID)

        ' Check for valid url
        If URL.Trim <> String.Empty Then
            ' Redirect
            Me.Response.Redirect(URL)
        Else
            ' Show error message 
            Me.ShowJavaMessage("Invalid URL!")
        End If
    End Sub

#End Region

#Region " ELABORATE "

    ' Generate a image from URL
    Private Function URLToBImage(URL As String, FileName As String) As String
        ' Create a path
        Dim Path As String = IO.Path.Combine(SCFramework.Bridge.Configuration.TemporaryPath, FileName & ".jpg")

        ' Check if already exists
        If Not IO.File.Exists(Path) Then
            Try
                ' Try to generate an image from URL
                Dim WTI As WebsiteToImage = New WebsiteToImage(URL, Path)
                WTI.Generate()

            Catch ex As Exception
                ' With error return nothing
                Return Nothing
            End Try
        End If

        ' If success return the path of the file
        Return Path
    End Function

    ' Elaborate the request
    Private Sub Elaborate()
        ' Hold the URL
        Dim URL As String = Me.TxtURL.Text

        ' Register the new URL and get the ID
        Dim ID As Long = Global.URLs.RegisterURL(URL)

        ' Create the code
        Dim Encoded As String = Global.Encoder.EncodeNumberToString(ID)

        ' Create a short URL and print at screen
        Dim ShortURL As String = String.Format("{0}/?{1}", SCFramework.Utils.GetAppURLDomain(), Encoded)
        Me.HLShortURL.Text = ShortURL
        Me.HLShortURL.NavigateUrl = ShortURL

        ' Show the result panel
        Me.PnlResult.Visible = True

        ' Get the creation date
        Dim CreationDate As Date = Global.URLs.GetCreationDate(ID)

        ' Set the panel controls
        If CreationDate > Date.MinValue Then Me.LitCreated.Text = CreationDate.ToString("f")
        Me.LitOriginal.Text = URL
        Me.TxtURL.Text = String.Empty

        ' Try to take a screen shot of the remote web page
        Dim Path As String = Me.URLToBImage(URL, Encoded)

        ' Check the result
        Me.ImgScreenShot.Visible = False
        If Path IsNot Nothing AndAlso IO.File.Exists(Path) Then
            ' Create a relative path to file
            Dim Relative As String = String.Format("~/{0}/{1}.jpg", SCFramework.Bridge.Configuration.TemporaryFolder, Encoded)
            ' Apply to image
            Me.ImgScreenShot.Visible = True
            Me.ImgScreenShot.ImageUrl = Relative
        End If
        Me.LblNoPreview.Visible = Not Me.ImgScreenShot.Visible
    End Sub

#End Region

#Region " EVENTS "

    ' Check the input url
    Private Sub CheckFields()
        ' Check for empty values
        If Not SCFramework.Utils.IsValidURL(Me.TxtURL.Text) Then
            ' Trhow an exception
            Throw New Exception("Its not valid URL!")
        End If
    End Sub

    ' Manager the action event
    Protected Sub BtnAction_Click(sender As Object, e As EventArgs) Handles BtnAction.Click
        ' Check for field
        Try
            ' Start transaction
            Me.Query.StartTransaction()

            ' Typed check
            Me.CheckFields()

            ' Try to elaborate the request
            Me.Elaborate()

            ' Commit
            Me.Query.CommitTransaction()

        Catch ex As Exception
            ' Rool back
            Me.Query.RoolBackTransaction()

            ' Show message at screen
            Me.AddAlertMessage(ex.Message)
        End Try
    End Sub

#End Region

End Class
