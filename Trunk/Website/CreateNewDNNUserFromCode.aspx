<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Import Namespace="DotNetNuke.Security.Membership" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="font-family: Arial">
    <form id="form1" runat="server">
    <div>
        <h2>
            <asp:Label ID="Label6" runat="server" Text="Create New User "></asp:Label>
            <asp:HyperLink  ID="loginLink" Text="(Login)" runat="server"/>
        </h2>
        <table border="0" cellpadding="3">
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Name"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TextBox_AddUser_Name" Text="Test" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="User"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TextBox_AddUser_Surname" Text="Test" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" Text="Email"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TextBox_AddUser_Email" Text="test@localhost.com" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label4" runat="server" Text="Username"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TextBox_AddUser_Username" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label5" runat="server" Text="Password"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TextBox_AddUser_Password" TextMode="Password" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label7" runat="server" Text="SuperUser"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="CheckBox_SuperUser" Checked="True" runat="server"></asp:CheckBox>
                </td>
            </tr>
        </table>
        <asp:Button ID="Button1" runat="server" Text="Create User" OnClick="Button1_Click" />
        <em>
            <asp:Label ID="Message" runat="server" Text=""></asp:Label>
        </em>
    </div>
    </form>
</body>
</html>
<script runat="server">

    protected override void OnLoad(EventArgs e)
    {
        PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
        loginLink.NavigateUrl = portalSettings.STDURL + "login.aspx";
    }

    protected void Button1_Click(object sender, System.EventArgs e)
    {
        try
        {
            //make sure to add user to "right" portal
            PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
            DotNetNuke.Entities.Users.UserInfo oUser = new DotNetNuke.Entities.Users.UserInfo();

            oUser.PortalID = portalSettings.PortalId;
            oUser.IsSuperUser = false;
            oUser.FirstName = TextBox_AddUser_Name.Text;
            oUser.LastName = TextBox_AddUser_Surname.Text;
            oUser.Email = TextBox_AddUser_Email.Text;
            oUser.Username = TextBox_AddUser_Username.Text;
            oUser.DisplayName = TextBox_AddUser_Name.Text;
            oUser.IsSuperUser = CheckBox_SuperUser.Checked;
            oUser.Profile.PreferredLocale = portalSettings.DefaultLanguage;
            oUser.Profile.TimeZone = portalSettings.TimeZoneOffset;
            oUser.Profile.FirstName = oUser.FirstName;
            oUser.Profile.LastName = oUser.LastName;

            oUser.Membership.Approved = true;
            oUser.Membership.CreatedDate = System.DateTime.Now;
            oUser.Membership.IsOnLine = false;
            oUser.Membership.Password = TextBox_AddUser_Password.Text;


            UserCreateStatus userCreateStatus = DotNetNuke.Entities.Users.UserController.CreateUser(ref oUser);
            
            Message.Text = userCreateStatus.ToString();

            if (userCreateStatus != UserCreateStatus.Success)
            {
                Message.ForeColor = Color.Red;
            }
            else
            {
                Message.ForeColor = Color.Green;
            }

        }
        catch (Exception ex)
        {
            Message.Text = ex.ToString();
        }

    }
</script>
