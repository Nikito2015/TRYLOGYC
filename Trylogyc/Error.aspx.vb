Public Class _Error
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.Label1.Text = Session("codError")
            Me.Label2.text = Session("txtError")
        End If
    End Sub

End Class