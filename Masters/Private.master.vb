
Partial Class Masters_Private
    Inherits SCFramework.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Me.PnlContent.Visible = Me.Page.CurrentUser.IsAutenticated
            Me.PnlLogin.Visible = Not Me.Page.CurrentUser.IsAutenticated

            Me.LblUserInfo.Visible = Me.PnlContent.Visible
            Me.LBLogout.Visible = Me.PnlContent.Visible

            Me.AddGlobalLoadingPanel()

            If [Page].CurrentUser.IsAutenticated And Not IsPostBack Then
                ' Menu
                Me.LoadPagesList()

                ' Breadcrumbs
                Me.InitBreadcrumbs()
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

#Region " BREADCRUMBS "

    Private Sub InitBreadcrumbs()
        ' Links
        Dim HTML As String = SCFramework.HTML.ControlToHtml(Me.NavMenuItems)
        Dim Links As ArrayList = SCFramework.HTML.GetLinksByHTML(HTML)

        For Each Link As String In Links
            Me.Page.Navigation.AddToBaseLinks(Link)
        Next

        ' Base page
        [Page].Navigation.SetBasePage("Desktop", "~/private/StartPage.aspx")
    End Sub

    Private Sub LoadBreadcrumbs()
        Dim ToRemove As String = SCFramework.Bridge.Configuration.ApplicationName & " - "
        Dim HTML As String = Me.Page.Navigation.ToBoostrap()

        Me.LitBreadcrumbs.Text = HTML.Replace(ToRemove, String.Empty)
    End Sub

#End Region

#Region " DESCRIPTION "

    Public Property PageTitleContent() As String
        Get
            Return ViewState("PageTitleContent")
        End Get
        Set(ByVal value As String)
            ViewState("PageTitleContent") = value

            Me.LblPageTitle.Text = "<h1>" & value & "</h1>"
            Me.LblPageTitle.Visible = Not String.IsNullOrEmpty(value)

            Me.Page.Header.Title = SCFramework.Bridge.Configuration.ApplicationName & " - " & value
        End Set
    End Property

    Public Property PageDescriptionContent() As String
        Get
            Return ViewState("PageDescriptionContent")
        End Get
        Set(ByVal value As String)
            ViewState("PageDescriptionContent") = value
            Me.LblPageDescripiton.Text = "<p>" & value & "</p>"
            Me.LblPageDescripiton.Visible = Not String.IsNullOrEmpty(value)
        End Set
    End Property

#End Region

#Region " NOTIFICATION "

    Private NotificationList As ArrayList = New ArrayList()

    Public Sub AddNotification(Message As String)
        NotificationList.Add(Message)
    End Sub

    Private Sub DrawNotification(Message As String, Index As Integer)
        Dim RN As Telerik.Web.UI.RadNotification = New Telerik.Web.UI.RadNotification()
        RN.ID = "RN" & Index.ToString
        RN.Width = Unit.Pixel(500)
        RN.Height = Unit.Pixel(110)
        RN.VisibleTitlebar = True
        RN.Animation = Telerik.Web.UI.NotificationAnimation.Fade
        RN.EnableRoundedCorners = True
        RN.TitleIcon = ""
        RN.ContentIcon = "warning"
        RN.AutoCloseDelay = 5000 + Index * 100
        RN.OffsetY = (-110 * Index)
        RN.ContentScrolling = Telerik.Web.UI.NotificationScrolling.None
        RN.Text = Message

        Me.form1.Controls.Add(RN)
        RN.Show()
    End Sub

    Private Sub DrawNotifications()
        If SCFramework.Bridge.CurrentUser.IsAdministrator Then
            For Index As Integer = 0 To NotificationList.Count - 1
                Dim Message As String = NotificationList(Index)
                Me.DrawNotification(Message, Index)
            Next
        End If
    End Sub

#End Region

#Region " EVENTS "

    Protected Sub RAPMainMenu_AjaxRequest(sender As Object, e As Telerik.Web.UI.AjaxRequestEventArgs) Handles RAPMainMenu.AjaxRequest
        Me.LoadPagesList()
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        Me.PageHeaderContent.Visible = Not String.IsNullOrEmpty(Me.LblPageTitle.Text)
        Me.DrawNotifications()

        If Not Me.Page.CurrentUser.IsAutenticated Then
            ' Title
            Me.PageTitleContent = "Login"
            Me.PageDescriptionContent = "Amministrazione"
        Else
            ' Infos
            Me.LblUserInfo.Text = "Welcome back <strong>" & Me.Page.CurrentUser.Login & "</strong>"

            ' Breadcrumbs
            Me.LoadBreadcrumbs()
        End If
    End Sub

    Protected Sub BtnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnLogin.Click
        Try
            If Me.Page.Login(Me.TxtAlias.Text, Me.TxtPassword.Text) Then
                Select Case Me.Page.CurrentUser.Level
                    Case SCFramework.UserInfo.Levels.Administrator, _
                         SCFramework.UserInfo.Levels.Manager
                        Me.Page.ForceReload()

                    Case Else
                        Me.Page.CurrentUser = New SCFramework.UserInfo()
                        Response.Redirect("../" & SCFramework.Bridge.Configuration.BasePage)

                End Select

            End If
        Catch ex As Exception
            Me.Page.ShowJavaMessage(ex.Message)
        End Try
    End Sub

    Protected Sub LBLogout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LBLogout.Click
        Try
            Me.Page.Logout()
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region " FILTERS "

    Private Function IsValidFiltersControl([Control] As Control) As Boolean
        Return (TypeOf [Control] Is TextBox) Or (TypeOf [Control] Is CheckBox) Or _
               (TypeOf [Control] Is RadioButton) Or (TypeOf [Control] Is DropDownList) Or _
               (TypeOf [Control] Is ListControl)
    End Function

    Private Sub SetControlValue(ByVal [Control] As Control, ByVal Value As String)
        If TypeOf [Control] Is TextBox Then
            CType([Control], TextBox).Text = Value

        ElseIf TypeOf [Control] Is CheckBox Then
            CType([Control], CheckBox).Checked = IIf(String.IsNullOrEmpty(Value), False, Value)

        ElseIf TypeOf [Control] Is RadioButton Then
            CType([Control], RadioButton).Checked = IIf(String.IsNullOrEmpty(Value), False, Value)

        ElseIf TypeOf [Control] Is ListControl Then
            CType([Control], ListControl).ClearSelection()

        End If
    End Sub

    Private Sub ResetControlValue(ByVal [Control] As Control)
        Me.SetControlValue([Control], Nothing)
    End Sub

    Private Sub ResetFilters(Parent As Control)
        If Parent.HasControls Then
            For Each [Control] As Control In Parent.Controls
                If Me.IsValidFiltersControl([Control]) Then
                    Me.ResetControlValue([Control])
                Else
                    Me.ResetFilters([Control])
                End If
            Next
        End If
    End Sub

    Dim ToUpdatesHolder() As Control = Nothing

    Private Sub SetFiltersControls()
        ' Visibility
        Me.PnlFilters.Visible = True

        ' Loading
        Me.BtnApplyFilters.Attributes("data-loading-text") = "Attendere ..."
        Me.BtnApplyFilters.OnClientClick = "$(this).button('loading');"

        Me.BtnResetFilters.Attributes("data-loading-text") = "Attendere ..."
        Me.BtnResetFilters.OnClientClick = "$(this).button('loading');"
    End Sub

    Public Sub ShowFiltersPanel(ToUpdate As Control, Callback As EventHandler)
        Dim Manager As Telerik.Web.UI.RadAjaxManager = Telerik.Web.UI.RadAjaxManager.GetCurrent(Me.Page)
        If Manager IsNot Nothing Then
            Me.SetFiltersControls()

            ' Ajax
            Manager.AjaxSettings.AddAjaxSetting(Me.BtnResetFilters, Me.PnlFiltersUpdateContent)
            Manager.AjaxSettings.AddAjaxSetting(Me.BtnApplyFilters, ToUpdate)

            ' Handler
            AddHandler BtnApplyFilters.Click, Callback
        End If
    End Sub

    Public Sub AddToAjaxManager(Control As Control)
        Dim Manager As Telerik.Web.UI.RadAjaxManager = Telerik.Web.UI.RadAjaxManager.GetCurrent(Me.Page)
        If Manager IsNot Nothing Then
            ' Ajax
            Manager.AjaxSettings.AddAjaxSetting(Control, Me.PnlFiltersUpdateContent)
        End If
    End Sub

    Public Sub ShowFiltersPanel(ParamArray ToUpdates() As Control)
        Dim Manager As Telerik.Web.UI.RadAjaxManager = Telerik.Web.UI.RadAjaxManager.GetCurrent(Me.Page)
        If Manager IsNot Nothing Then
            Me.SetFiltersControls()

            ' Ajax
            Manager.AjaxSettings.AddAjaxSetting(Me.BtnResetFilters, Me.PnlFiltersUpdateContent)

            ' Apply
            If ToUpdates IsNot Nothing Then
                For Each [Control] As Control In ToUpdates
                    Manager.AjaxSettings.AddAjaxSetting(Me.BtnApplyFilters, [Control])
                Next
            End If

            ' Holder
            ToUpdatesHolder = ToUpdates

            ' Handler
            AddHandler BtnApplyFilters.Click, AddressOf BtnApplyFilters_Click
        End If
    End Sub

    Protected Sub BtnResetFilters_Click(sender As Object, e As System.EventArgs) Handles BtnResetFilters.Click
        Me.ResetFilters(Me.PnlFiltersUpdateContent)
    End Sub

    Protected Sub BtnApplyFilters_Click(sender As Object, e As System.EventArgs)
        If ToUpdatesHolder IsNot Nothing Then
            For Each [Control] As Control In ToUpdatesHolder
                If TypeOf [Control] Is Telerik.Web.UI.RadGrid Then
                    CType([Control], Telerik.Web.UI.RadGrid).Rebind()
                End If
            Next
        End If
    End Sub

#End Region

#Region " PAGES MENU "

    Private Function CreatePageItem(Source As DataTable, Current As DataRow) As String
        Dim HTML As String = String.Empty
        Dim Link As String = String.Format("../private/PageDetails.aspx?ID={0}", Current!ID_PAGE)

        If Not IsDBNull(Current!INTERNAL_URL) Then
            Link = String.Format("{0}?ID={1}", Current!INTERNAL_URL, Current!ID_PAGE)
        End If

        Dim DV As DataView = New DataView(Source)
        DV.Sort = "[ORDER_INDEX]"
        DV.RowFilter = "[ID_PARENT] = " & Current!ID_PAGE

        If DV.Count > 0 Then
            HTML &= "<li class='dropdown-submenu'>"
            HTML &= String.Format("<a class='dropdown-toggle' data-toggle='dropdown' href='{0}'>{1}</a>", Link, "" & Current!DEFTITLE)
            HTML &= "<ul class='dropdown-menu'>"

            For Each Row As DataRowView In DV
                HTML &= Me.CreatePageItem(DV.Table, Row.Row)
            Next

            HTML &= "</ul>"
            HTML &= "</li>"
        Else
            HTML &= String.Format("<li><a href='{0}'>{1}</a></li>", Link, "" & Current!DEFTITLE)
        End If

        Return HTML
    End Function

    Private Function CreatePageList(Source As DataView) As String
        Dim DV As DataView = New DataView(Source.Table)
        DV.Sort = "[ORDER_INDEX]"
        DV.RowFilter = "[ID_PARENT] IS NULL"

        Dim HTML As String = IIf(DV.Count > 0 And SCFramework.Bridge.CurrentUser.IsRoot, "<li class='divider'></li>", String.Empty)
        For Each Row As DataRowView In DV
            HTML &= Me.CreatePageItem(Source.Table, Row.Row)
        Next

        Return HTML
    End Function

    Private Sub LoadPagesList()
        Dim Filters As Hashtable = New Hashtable()
        Filters.Add("SHOW_INPRIVATE", True)

        Dim Source As DataView = Me.Page.PagesManager.GetSource(Filters)

        Me.LitPagesList.Text = Me.CreatePageList(Source)
        Me.LitPagesList.Visible = Not String.IsNullOrEmpty(Me.LitPagesList.Text)
    End Sub

#End Region

#Region " LOADING PANEL "

    Public Sub AddGlobalLoadingPanel(Optional Target As Telerik.Web.UI.RadAjaxPanel = Nothing)
        If Target Is Nothing Then
            Dim RAM As Telerik.Web.UI.RadAjaxManager = Telerik.Web.UI.RadAjaxManager.GetCurrent(Me.Page)
            If RAM IsNot Nothing Then
                RAM.ClientEvents.OnRequestStart = "showLoadingPanel"
                RAM.ClientEvents.OnResponseEnd = "hideLoadingPanel"
            End If
        Else
            Target.ClientEvents.OnRequestStart = "showLoadingPanel"
            Target.ClientEvents.OnResponseEnd = "hideLoadingPanel"
        End If
    End Sub

#End Region

End Class

