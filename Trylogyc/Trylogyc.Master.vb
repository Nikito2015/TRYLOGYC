Imports System.Collections.Generic
Imports Trylogyc.Models
Imports Trylogyc.DAL
Imports System.Data.Entity.Migrations
Imports System.Linq
Imports Trylogyc.Models.Usuario
Public Class Trylogyc
    Inherits System.Web.UI.MasterPage

    Private _Usuario As Usuario
    Public Property Usuario() As Usuario
        Get
            Return _Usuario
        End Get
        Set(ByVal value As Usuario)
            _Usuario = value
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If login.ValidateSessionCookie() = True Then
            'Dim context = New TrylogycContext
            'Dim dsUsuario As New DataSet
            'Dim dsCops As New DataSet
            'dsUsuario = context.GetUsuario(Session("IDUsuario"))
            'dsCops = context.GetCops(Session("IDUsuario"))
            'Dim miUsuario = New Usuario
            'miUsuario.IDUsuario = Session("IDUsuario")
            'miUsuario.IDUsuario = Me.Context.Items("IDUsuario").ToString()
            'miUsuario.XmlSocio = Session("xmlSocio")
            'miUsuario.XmlSocio = Me.Context.Items("xmlSocio").ToString()
            'miUsuario.email = Session("userEmail")
            'miUsuario.email = Me.Context.Items("userEmail").ToString()
            'miUsuario.username = Session("nomUsuario")
            'miUsuario.username = Me.Context.Items("nomUsuario").ToString()
            'miUsuario.passWord = Session("Password")
            'miUsuario.passWord = Me.Context.Items("Password").ToString()
            'miUsuario.foto = Session("Foto")
            'miUsuario.foto = Me.Context.Items("Foto").ToString()
            'Me.lblcliente.Text = miUsuario.username
            'Me.fotocliente2.ImageUrl = miUsuario.foto
            'Me.lblcliente2.Text = miUsuario.username

            'Me.lblco1.Text = Session("conCount")
            'Me.lblco1txt.Text = "Conexiones"

        Else
            Response.Redirect("~/login.aspx")
        End If


    End Sub

End Class