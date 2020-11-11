Imports System.Collections.Generic
Imports Trylogyc.Models
Imports Trylogyc.DAL
Imports System.Data.Entity.Migrations
Imports System.Linq
Imports Trylogyc.Models.Usuario
Public Class Trylogyc
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If login.ValidateSessionCookie() = True Then
            Dim context = New TrylogycContext
            Dim dsUsuario As New DataSet
            Dim dsCops As New DataSet
            If Session("IDUsuario") Is Nothing Then
                Session("IDUsuario") = Request.Cookies("IDUsuario")?.Value
            End If
            If Not Session("IDUSuario") Is Nothing Then
                dsUsuario = context.GetUsuario(Session("IDUsuario"))
                dsCops = context.GetCops(Session("IDUsuario"))
                Dim miUsuario = New Usuario
                miUsuario.IDUsuario = dsUsuario.Tables(0).Rows(0).Item(0)
                miUsuario.XmlSocio = dsUsuario.Tables(0).Rows(0).Item(1)
                miUsuario.email = dsUsuario.Tables(0).Rows(0).Item(2)
                miUsuario.username = dsUsuario.Tables(0).Rows(0).Item(3)
                miUsuario.passWord = dsUsuario.Tables(0).Rows(0).Item(4)
                miUsuario.foto = dsUsuario.Tables(0).Rows(0).Item(5)
                Me.lblcliente.Text = miUsuario.username
                Me.fotocliente2.ImageUrl = miUsuario.foto
                Me.lblcliente2.Text = miUsuario.username

                Me.lblco1.Text = Session("conCount")
            End If

            Me.lblco1txt.Text = "Conexiones"

        Else
            Response.Redirect("~/login.aspx")
        End If


    End Sub

End Class