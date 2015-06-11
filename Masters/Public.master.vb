
Partial Class Masters_Public
    Inherits SCFramework.MasterPage

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        ' Postback
        If Not IsPostBack Then
            ' DO NOTHING
        End If
    End Sub

End Class

