Public Class ConfirmarPago
    Inherits System.Web.UI.Page

    Private _numFact As String
    Public Property numFact() As String
        Get
            Return _numFact
        End Get
        Set(ByVal value As String)
            _numFact = value
        End Set
    End Property

    Private _strImport As String
    Public Property strImport() As String
        Get
            Return _strImport
        End Get
        Set(ByVal value As String)
            _strImport = value
        End Set
    End Property

    Private _idSocio As Int32
    Public Property idSocio() As Int32
        Get
            Return _idSocio
        End Get
        Set(ByVal value As Int32)
            _idSocio = value
        End Set
    End Property

    Private _idConexion As Int32
    Public Property idConexion() As Int32
        Get
            Return _idConexion
        End Get
        Set(ByVal value As Int32)
            _idConexion = value
        End Set
    End Property

    Private _IDUsuario As Int32
    Public Property IDUsuario() As Int32
        Get
            Return _IDUsuario
        End Get
        Set(ByVal value As Int32)
            _IDUsuario = value
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        nroFactura = Request.Params("numfact")
        strImport = Request.Params("importe")
        idSocio = Request.Params("idSocio")
        idConexion = Request.Params("idConexion")
    End Sub

    Protected Sub btnContinuar_Click(sender As Object, e As EventArgs) Handles btnContinuar.Click
        Dim nroFactura As String = Request.Params("numfact")
        Dim strImport As String = Request.Params("importe")
        Dim idSocio As Int32 = Request.Params("idSocio")
        Dim idConexion As Int32 = Request.Params("idConexion")
        Response.Redirect("~/Pagar.aspx?numfact=" & LTrim(RTrim(nroFactura)) & "&importe=" & strImport & "&idSocio=" & idSocio & "&idConexion=" & idConexion)
    End Sub
    Protected Sub btnVolver_Click(sender As Object, e As EventArgs) Handles btnVolver.Click
        Response.Redirect("~/Default.aspx?" & "IDUsuario=" & IDUsuario & "xmlSocio=" & idSocio)
    End Sub
End Class