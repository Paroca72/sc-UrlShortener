<%@ Page Title="" Language="VB" MasterPageFile="~/Masters/Public.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPHBody" runat="Server">

    <!-- Conversion Panel -->
    <section id="conversion" class="centered">
        <div>
            <div class="container">

                <!-- Result panel -->
                <asp:ScriptManager ID="MainScriptManager" runat="server"></asp:ScriptManager>
                <asp:UpdatePanel ID="UP" runat="server">
                    <ContentTemplate>

                        <!-- Request panel -->
                        <div class="row">
                            <div class="col-md-12">
                                <div class="default-box">
                                    <div class="form-group">
                                        <label for="exampleInputEmail1">Paste your long url here:</label>
                                        <div class="input-group">
                                            <asp:TextBox ID="TxtURL" runat="server" CssClass="form-control" placeholder="Enter long url"></asp:TextBox>
                                            <span class="input-group-btn">
                                                <asp:Button ID="BtnAction" runat="server" Text="Create" CssClass="btn btn-primary" />
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Result panel -->
                        <asp:Panel ID="PnlResult" runat="server" Visible="false" CssClass="row">
                            <div class="col-md-5">
                                <span>Details</span>
                                <div class=" default-box resume">
                                    <asp:HyperLink ID="HLShortURL" runat="server"></asp:HyperLink>
                                    <table class="table">
                                        <tr>
                                            <td>Created:</td>
                                            <td>
                                                <asp:Literal ID="LitCreated" runat="server"></asp:Literal>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Original:</td>
                                            <td>
                                                <asp:Literal ID="LitOriginal" runat="server"></asp:Literal>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                            <div class="col-md-6 col-md-offset-1">
                                <span>Preview</span>
                                <div class="default-box preview">
                                    <asp:Image ID="ImgScreenShot" runat="server" />
                                    <asp:Label ID="LblNoPreview" runat="server" Text="Label">Sorry! Preview not available</asp:Label>
                                </div>
                            </div>
                        </asp:Panel>

                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="BtnAction" />
                    </Triggers>
                </asp:UpdatePanel>

            </div>
        </div>
    </section>

</asp:Content>

