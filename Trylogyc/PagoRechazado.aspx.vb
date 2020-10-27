Imports Trylogyc.DAL
Imports Trylogyc.DAL.TrylogycContext

Public Class PagoRechazado
    Inherits System.Web.UI.Page

    Public IdComprobante As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim collection As String = Request.Params("collection_id")
            Dim merchantOrder As String = Request.Params("merchant_order_id")
            Dim preference As String = Request.Params("preference_id")
            Dim idUsuario As Int32 = Request.Cookies("IDUsuario").Value
            Dim xmlSocio As Int32 = Request.Cookies("xmlSocio").Value
            If Session("IDUsuario") Is Nothing Then
                Session("IDUsuario") = idUsuario
            End If
            If Session("xmlSocio") Is Nothing Then
                Session("xmlSocio") = xmlSocio
            End If
            Dim myContext As New TrylogycContext
            Dim pagoRegistrado As Boolean = myContext.ActualizarPagoPorPreferencia(preference, EstadosPagos.Rechazado, collection, merchantOrder)
        End If
    End Sub

End Class