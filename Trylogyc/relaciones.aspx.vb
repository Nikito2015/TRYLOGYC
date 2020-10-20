Imports Trylogyc.DAL
Imports System.Xml
Imports System.Linq
Imports System.IO
Public Class relaciones
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.txtcgp.Text = ""
            Me.txtcodsocio.Text = ""
            Me.divError.Visible = False
            Me.divSuccess.Visible = False
            Me.Form.DefaultButton = Me.btnrelacionar.UniqueID
        End If
    End Sub

    Protected Sub btnrelacionar_Click(sender As Object, e As EventArgs) Handles btnrelacionar.Click
        Dim myContext As New TrylogycContext

        Dim codigo = Me.txtcodsocio.Text
        Dim cgp = Me.txtcgp.Text

        If Len(codigo) = 10 Then
            Dim IDSocio As Int32
            Dim IDConexion As Int32
            'Dim cntSocio As Int32
            IDSocio = Convert.ToInt32(Mid(Me.txtcodsocio.Text, 1, 6))
            IDConexion = Convert.ToInt32(Mid(Me.txtcodsocio.Text, 8, 4))
            'ir a la base y revisar que ese xmlSocio no tenga un usuario ya asignado.
            Dim checkSocio = myContext.GetSocios() 'validar que el nro. de socio ingresado corresponda a un socio
            If checkSocio.Tables(0).TableName = "Error" Then
                Session("codError") = checkSocio.Tables(0).Rows(0).Item(0)
                Session("txtError") = checkSocio.Tables(0).Rows(0).Item(1)
                Me.lblError.Text = "Error" & checkSocio.Tables(0).Rows(0).Item(0)
            Else
                Dim encontrado = False
                If checkSocio.Tables(0) IsNot Nothing Then
                    For Each r In checkSocio.Tables(0).Rows
                        If Convert.ToInt32(r.Item(0)) = IDSocio And
                                Convert.ToInt32(r.Item(1)) = IDConexion Then
                            encontrado = True
                            If r.Item(2) = cgp Then 'Para dicho socio y conexion que coincida el CGP
                                'registrar relacion
                                Dim dsnewrel As DataSet
                                dsnewrel = myContext.PutRelacion(Session("IDUsuario"), IDSocio, IDConexion)

                                If dsnewrel.Tables.Count > 0 Then
                                    divSuccess.Visible = False
                                    divError.Visible = True
                                    lblError.Text = "No se ha podido completar la operación"

                                Else
                                    divSuccess.Visible = True
                                    divError.Visible = False
                                    lblSuccess.Text = "Relación Registrada Exitosamente. Vuelva a Ingresar al Sitio para ver los Cambios"

                                End If
                            Else
                                divSuccess.Visible = False
                                divError.Visible = True
                                lblError.Text = "Los datos ingresados son incorrectos"
                            End If
                        End If


                    Next
                    If encontrado = False Then
                        divSuccess.Visible = False
                        divError.Visible = True
                        lblError.Text = "Los datos ingresados son incorrectos"
                    End If
                Else
                    divSuccess.Visible = False
                    divError.Visible = True
                    lblError.Text = "Los datos ingresados son incorrectos"
                End If
            End If
        Else
            divSuccess.Visible = False
            divError.Visible = True
            lblError.Text = "Los datos ingresados son incorrectos"

        End If
    End Sub

    Protected Sub btnVolver_Click(sender As Object, e As EventArgs) Handles btnVolver.Click
        If lblSuccess.Text = "Relación Registrada Exitosamente. Vuelva a Ingresar al Sitio para ver los Cambios" Then
            Response.Redirect("~/login.aspx")
        Else
            Response.Redirect("~/Default.aspx")
        End If
    End Sub
End Class