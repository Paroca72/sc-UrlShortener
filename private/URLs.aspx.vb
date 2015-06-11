'------------------------------------------------------------------
' PAGE FOR MANAGE URLS
'------------------------------------------------------------------

Partial Class private_URLs
    Inherits SCFramework.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        ' If not is post back
        If Not IsPostBack Then
            ' Set the page information
            Me.Master.PageTitleContent = "URLs management"
            Me.Master.PageDescriptionContent = "Administration"

            ' Set the base controls
            Me.SetControls()
            ' Load the grid
            Me.GridURLs.DataBind()
        End If
    End Sub

#Region " COMMON "

    ' Set all controls that need to be setting
    Private Sub SetControls()
        ' Grids
        Me.GridURLs.Skin = Global.InternalStyles.TelerikGridStyle
    End Sub

#End Region

#Region " GRID "

    ' When need to reload datasource and bind with grid
    Protected Sub GridURLs_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles GridURLs.NeedDataSource
        ' Load source
        Dim Table As DataTable = Global.URLs.GetSource()
        ' Apply to grid
        Me.GridURLs.DataSource = Table
    End Sub

    ' On data binding
    Protected Sub GridURLs_ItemDataBound(sender As Object, e As Telerik.Web.UI.GridItemEventArgs) Handles GridURLs.ItemDataBound
        ' Only if in insert or edit mode
        If TypeOf (e.Item) Is Telerik.Web.UI.GridEditableItem Then
            ' Get the current row
            Dim Row As DataRowView = TryCast(e.Item.DataItem, DataRowView)
            ' Get the current item
            Dim Item As Telerik.Web.UI.GridEditableItem = CType(e.Item, Telerik.Web.UI.GridEditableItem)

            ' Edit mode
            If e.Item.IsInEditMode Then
                ' Flag for understand if is in insert mode
                Dim InsertMode As Boolean = TypeOf Item Is Telerik.Web.UI.GridDataInsertItem

                ' Text box features
                Dim Name As TextBox = Item("NameColumn").Controls(0)
                Name.Width = Unit.Percentage(100)

                ' Delete button features
                Dim DeleteCell As TableCell = Item("DeleteColumn")
                DeleteCell.Enabled = Not InsertMode
            End If

            ' Delete button features
            Dim Script As String = "return confirm('Are you sure?');"
            Dim LBDelete As LinkButton = Item("DeleteColumn").FindControl("LBDelete")
            LBDelete.OnClientClick = Script
        End If
    End Sub

    ' On delete grid row
    Protected Sub GridURLs_DeleteCommand(sender As Object, e As Telerik.Web.UI.GridCommandEventArgs) Handles GridURLs.DeleteCommand
        Try
            ' Get the database ID of the row
            Dim ID As Long = CType(e.Item, Telerik.Web.UI.GridDataItem).GetDataKeyValue("ID_URL")

            ' Manage transaction and delete the database row
            Me.Query.StartTransaction()
            Global.URLs.DeleteURL(ID)
            Me.Query.FinishTransaction(True)

        Catch ex As Exception
            ' Manage transaction and show alert message to user
            Me.Query.FinishTransaction(False)
            Global.Common.AddAlertMessage(Me.RadAjaxManager1, ex.Message)
            e.Canceled = True
        End Try
    End Sub

    ' On insert grid row
    Protected Sub GridURLs_InsertCommand(sender As Object, e As Telerik.Web.UI.GridCommandEventArgs) Handles GridURLs.InsertCommand
        Try
            ' Get the item
            Dim Item As Telerik.Web.UI.GridDataItem = CType(e.Item, Telerik.Web.UI.GridDataItem)

            ' Extract all values
            Dim Values As Hashtable = New Hashtable()
            Item.ExtractValues(Values)

            ' Get the URL
            Dim URL As String = Values("URL")

            ' Manage transaction and save the database row
            Me.Query.StartTransaction()
            Global.URLs.RegisterURL(URL)
            Me.Query.FinishTransaction(True)

        Catch ex As Exception
            ' Manage transaction and show alert message to user
            Me.Query.FinishTransaction(False)
            Global.Common.AddAlertMessage(Me.RadAjaxManager1, ex.Message)
            e.Canceled = True
        End Try
    End Sub

    ' On update grid row
    Protected Sub GridURLs_UpdateCommand(sender As Object, e As Telerik.Web.UI.GridCommandEventArgs) Handles GridURLs.UpdateCommand
        Try
            ' Get the item
            Dim Item As Telerik.Web.UI.GridDataItem = CType(e.Item, Telerik.Web.UI.GridDataItem)
            ' Get the ID of database row
            Dim ID As Long = Item.GetDataKeyValue("ID_URL")

            ' Extract all values
            Dim Values As Hashtable = New Hashtable()
            Item.ExtractValues(Values)

            ' Get the URL
            Dim URL As String = Values("URL")

            ' Manage transaction and save the database row
            Me.Query.StartTransaction()
            Global.URLs.UpdateURL(ID, Values("URL"))
            Me.Query.FinishTransaction(True)

        Catch ex As Exception
            ' Manage transaction and show alert message to user
            Me.Query.FinishTransaction(False)
            Global.Common.AddAlertMessage(Me.RadAjaxManager1, ex.Message)
            e.Canceled = True
        End Try
    End Sub

#End Region

End Class
