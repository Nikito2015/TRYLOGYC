Imports Trylogyc.DAL
Imports Trylogyc.DAL.TrylogycContext

Public Class PagoExitoso
    Inherits System.Web.UI.Page

    Public IdComprobante As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim collection As String = Request.Params("collection_id")
            Dim merchantOrder As String = Request.Params("merchant_order_id")
            Dim preference As String = Request.Params("preference_id")
            Dim idUsuario As Int32 = Convert.ToInt32(Request.Params("IDUsuario"))
            If Session("IDUsuario") Is Nothing Then
                Session("IDUsuario") = idUsuario
            End If
            Dim myContext As New TrylogycContext
            Dim pagoRegistrado As Boolean = myContext.ActualizarPagoPorPreferencia(preference, EstadosPagos.Aprobado, collection, merchantOrder)
            lblComprobante.Text = collection
        End If

    End Sub

End Class