Imports Trylogyc.DAL
Imports System.Xml
Imports System.Linq
Imports System.IO
Imports Trylogyc.Models
Public Class UsuarioModif
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        divError.Visible = False
        divSuccess.Visible = False
        If Not Page.IsPostBack Then
            aceptaEmail.Checked = Convert.ToBoolean(Session("aceptaFacturaMail"))

        End If

    End Sub

    Protected Sub btnSavePass_Click(sender As Object, e As EventArgs) Handles btnSavePass.Click
        Dim myContext As New TrylogycContext
        Dim daUser As DataSet = myContext.GetUsuarioNombre(Session("nomUsuario"))
        Dim myUser = New Usuario
        If daUser IsNot Nothing Then
            If daUser.Tables(0).TableName = "Error" Then
                lblError.Text = "Error: " & daUser.Tables(0).Rows(0).Item(0)
                divError.Visible = True
                divSuccess.Visible = False
            Else
                If daUser.Tables(0).Rows.Count > 0 Then 'Existe un usuario para dicho codigo de socio
                    myUser.passWord = daUser.Tables(0).Rows(0).Item("userPass")
                    myUser.IDUsuario = daUser.Tables(0).Rows(0).Item("IDUsuario")

                    myUser.EnviarFacturaEmail = Me.aceptaEmail.Checked

                    If String.IsNullOrEmpty(txtpassWordOld.Text) AndAlso
                                String.IsNullOrEmpty(txtPassWordNew.Text) AndAlso
                                    String.IsNullOrEmpty(txtPassWordNewCnf.Text) Then
                        'No modificó los datos ni ingresó password
                        Me.txtpassWordOld.Text = myUser.passWord
                        Me.txtPassWordNew.Text = myUser.passWord
                        Me.txtPassWordNewCnf.Text = myUser.passWord
                    End If

                    If Me.txtpassWordOld.Text = myUser.passWord Then 'la contraseña coincide
                        If Me.txtPassWordNew.Text = Me.txtPassWordNewCnf.Text And Len(Me.txtPassWordNew.Text) > 5 Then
                            myUser.passWord = Me.txtPassWordNew.Text
                            Dim daPass As DataSet = myContext.UpdUsuario(myUser)
                            If daPass IsNot Nothing Then
                                If daUser.Tables(0).TableName = "Error" Then
                                    lblError.Text = "Error: " & daUser.Tables(0).Rows(0).Item("IDUsuario")
                                    divError.Visible = True
                                    divSuccess.Visible = False
                                Else
                                    Session("aceptaFacturaMail") = aceptaEmail.Checked
                                    divError.Visible = False
                                    divSuccess.Visible = True
                                    lblSuccess.Text = "Datos actualizados exitosamente"
                                    txtPassWordNew.Text = ""
                                    txtPassWordNewCnf.Text = ""
                                    txtpassWordOld.Text = ""
                                End If
                            End If
                        Else
                            lblError.Text = "La contraseña debe tener al menos 6 caracteres alfanuméricos"
                            divError.Visible = True
                            divSuccess.Visible = False
                        End If

                    Else
                        lblError.Text = "La contraseña no es correcta"
                            divError.Visible = True
                            divSuccess.Visible = False
                        End If
                    Else  'Chequear si el codigo de socio ingresado existe en listado de socios, caso afirmativo redireccionar a registro.
                        lblError.Text = "No existe Usuario"
                    divError.Visible = True
                    divSuccess.Visible = False
                End If
            End If


        End If
    End Sub

    Protected Sub btnVolver_Click(sender As Object, e As EventArgs) Handles btnVolver.Click
        Response.Redirect("~/Default.aspx")
    End Sub
End Class