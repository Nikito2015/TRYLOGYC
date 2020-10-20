Public Class showPdf
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Headers.Remove("Server")
        iframepdf.Attributes("src") = "~/_tmp/" & Session("filename")
    End Sub

End Class