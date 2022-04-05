var token;

window.onload = function () {
    InitTabEvent();
    token = Cookies.get("JwtToken");
    let url = new URL(this.location.href);

    if (url.searchParams.has('code') && url.searchParams.has('state')) {
        let state = url.searchParams.get('state');
        if (state != Cookies.get("ConnectState")) {
            bootbox.alert({
                message: "state驗證失敗"
            });
            return;
        }

        let stateArr = state.split('_');
        switch (stateArr[0]) {
            case 'LineNotify':
                SubscribeLineNotify(url.searchParams.get('code'));
                break;
            case 'FacebookLogin':
            case 'LineLogin':
                OAuthLoginConnect(stateArr[0], url.searchParams.get('code'));
                break;
        }
    }

    InitUserProfile();
}

function InitTabEvent() {
    $('#myTab a').on('click',
        function (event) {
            event.preventDefault();
            $(this).tab('show');

            if (event.target.id == 'users-tab') {
                initUserList();
            }
        });
}

/**
 * 初始化個人資料
 */
var InitUserProfile = function () {
    let getUrl = `${settings.BaseDomainApiUrl}/User/GetSelf`;
    $.ajax({
        method: "GET",
        url: getUrl,
        dataType: "json",
        headers: {
            'Authorization': `Bearer ${token}`
        },
        success: function (result) {
            $('#PhotoUrl').attr('src', result.PhotoUrl);
            $('#Name').val(result.Name);
            $('#Account').val(result.Account);
            $('#Description').val(result.Description);
            $('#Email').val(result.Email);

            handleOAuthSetting(result);

            $('#Loading').hide();
        },
        error: function (result) {
            if (result.status === 401) window.location.href = "/";
        }
    });
}

/**
 *  初始化使用者列表
 * @returns {} 
 */
initUserList = function () {
    $('#Loading').show();
    let getUrl = `${settings.BaseDomainApiUrl}/User/GetUserList`;
    $.ajax({
        method: "GET",
        url: getUrl,
        dataType: "json",
        headers: {
            'Authorization': `Bearer ${token}`
        },
        success: function (users) {

            let dynamicTemplate = '';
            users.forEach(user => {

                let buttonTemplate = user.UserOAuthData.findIndex(x => x.ProviderType == 'LineNotify') > -1
                    ?
                    (`<button type = "button" class="btn btn-outline-success" onclick= "SendMessageConfirm(${user.UserId})" >` +
                        '<svg xmlns="http://www.w3.org/2000/svg" width="1rem" height="1rem" fill="currentColor" class="bi bi-bell text-success" viewBox="0 0 16 16">' +
                        '<path d="M8 16a2 2 0 0 0 2-2H6a2 2 0 0 0 2 2zM8 1.918l-.797.161A4.002 4.002 0 0 0 4 6c0 .628-.134 2.197-.459 3.742-.16.767-.376 1.566-.663 2.258h10.244c-.287-.692-.502-1.49-.663-2.258C12.134 8.197 12 6.628 12 6a4.002 4.002 0 0 0-3.203-3.92L8 1.917zM14.22 12c.223.447.481.801.78 1H1c.299-.199.557-.553.78-1C2.68 10.2 3 6.88 3 6c0-2.42 1.72-4.44 4.005-4.901a1 1 0 1 1 1.99 0A5.002 5.002 0 0 1 13 6c0 .88.32 4.2 1.22 6z" />' +
                        '</svg>' +
                        '私訊' +
                        '</button>')
                    : '';

                dynamicTemplate += '<li class="list-group-item d-flex justify-content-between align-items-center">' +
                    '<div class="col-10" style="text-align: start;">' +
                    `<img src="${user.PhotoUrl}" class="rounded-circle" style = "width: 3rem;">` +
                    `<span style="padding: 0rem 2rem;">${user.Name}</span> ` +
                    '</div>' +
                    buttonTemplate +
                    '</li >';
            });

            $('#userList').html(dynamicTemplate);
            $('#Loading').hide();
        },
        error: function (result) {
            if (result.status === 401) window.location.href = "/";
            $('#Loading').hide();
        }
    });

}

var handleOAuthSetting = function (result) {
    ResetOAuthSetting();

    result.UserOAuthData.forEach(item => {
        // 變更狀態
        $(`#${item.ProviderType}Status`).text(`已綁定(${item.ActiveDateTime})`);

        $(`#${item.ProviderType}ConnectBtn`).hide();
        $(`#${item.ProviderType}DisconnectBtn`).show();
    });
}

/**
 * 取得Line Notify 授權
 */
var UserLineNotifyAuth = function () {
    let state = `LineNotify_${Date.now()}`;
    Cookies.set("ConnectState", state);

    let oauthSetting = settings.OAuthSettings.find(x => x.ProviderType == 'LineNotify');
    let authUrl = 'https://notify-bot.line.me/oauth/authorize?';
    authUrl += 'response_type=code';
    authUrl += `&client_id=${oauthSetting.ClientId}`;
    authUrl += `&redirect_uri=${oauthSetting.RedirectUri}`;
    authUrl += `&state=${state}`;
    authUrl += '&scope=notify';
    window.location.href = authUrl;
}

/**
 * 訂閱Line Notify
 * @param {any} code
 */
var SubscribeLineNotify = function (code) {
    let postUrl = `${settings.BaseDomainApiUrl}/LineNotify/Subscribe`;
    let postData = { Code: code };

    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        headers: {
            'Authorization': `Bearer ${token}`
        },
        data: JSON.stringify(postData),
        dataType: "text",
        success: function () {
            bootbox.alert({
                message: "Line Notify訂閱成功",
                callback: function () {
                    window.location.href = '../Profile';
                }
            });
        },
        error: function () {
            bootbox.alert({
                message: "Line Notify訂閱失敗"
            });
        }
    });
}

/**
 * 取消訂閱Line Notify
 */
var UnSubscribeLineNotify = function () {
    let postUrl = `${settings.BaseDomainApiUrl}/LineNotify/UnSubscribe`;
    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        headers: {
            'Authorization': `Bearer ${token}`
        },
        dataType: "text",
        success: function () {
            bootbox.alert("Line Notify取消訂閱成功", function () {
                InitUserProfile();
            });
        },
        error: function () {
            bootbox.alert({
                message: "Line Notify取消訂閱失敗"
            });
        }
    });
}

/**
 *  取消OAuth Login 綁定
 * @param {any} providerType
 */
var OAuthLoginDisconnect = function (providerType) {
    let postUrl = `${settings.BaseDomainApiUrl}/Login/Disconnect/${providerType}`;
    $.ajax({
        method: "DELETE",
        url: postUrl,
        contentType: "application/json",
        headers: {
            'Authorization': `Bearer ${token}`
        },
        dataType: "text",
        success: function () {
            bootbox.alert("取消綁定成功", function () {
                InitUserProfile();
            });
        },
        error: function () {
            bootbox.alert({
                message: "取消綁定失敗"
            });
        }
    });
}

/**
 * 發送訊息確認
 * @param {any} userId 指定使用者 || 群發
 */
var SendMessageConfirm = function (userId) {
    bootbox.prompt({
        title: '發送訊息',
        value: `${userId ? '私訊' : '廣播'}:Hello World!`,
        callback: function (message) {
            if (message) {
                $('.bootbox-input-text').removeClass('is-invalid');
                SendMessage(userId, message);
            } else if (message === '') {
                $('.bootbox-input-text').addClass('is-invalid');
                return false;
            }
        }
    });
}

var SendMessage = function (userId, message) {

    let postUrl = `${settings.BaseDomainApiUrl}/LineNotify/Notify`;
    let postData = { UserId: userId, Message: message };

    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        headers: {
            'Authorization': `Bearer ${token}`
        },
        data: JSON.stringify(postData),
        dataType: "text",
        success: function () {
            bootbox.alert({
                message: "Line Notify發送訊息成功"
            });
        },
        error: function () {
            bootbox.alert({
                message: "Line Notify發送訊息失敗"
            });
        }
    });
}

var Logout = function () {
    window.location.href = "/";
}

var RedirectLineLoginConnect = function () {
    let state = `LineLogin_${Date.now()}`;
    Cookies.set("ConnectState", state);

    let oauthSetting = settings.OAuthSettings.find(x => x.ProviderType == 'LineLogin');
    let url = 'https://access.line.me/oauth2/v2.1/authorize?';
    url += 'response_type=code';
    url += `&client_id=${oauthSetting.ClientId}`;
    url += `&redirect_uri=${oauthSetting.ConnectRedirectUri}`;
    url += `&state=${state}`;
    url += '&scope=profile%20openid%20email';
    window.location.href = url;
}

var RedirectFacebookLoginConnect = function () {
    let state = `FacebookLogin_${Date.now()}`;
    Cookies.set("ConnectState", state);

    let oauthSetting = settings.OAuthSettings.find(x => x.ProviderType == 'FacebookLogin');
    let url = 'https://www.facebook.com/v13.0/dialog/oauth?';
    url += `&client_id=${oauthSetting.ClientId}`;
    url += `&redirect_uri=${oauthSetting.ConnectRedirectUri}`;
    url += `&state=${state}`;
    window.location.href = url;
}

OAuthLoginConnect = function (providerType, code) {
    let postUrl = `${settings.BaseDomainApiUrl}/Login/Connect`;
    let oauthSetting = settings.OAuthSettings.find(x => x.ProviderType == providerType);
    let postData = { ProviderType: providerType, Code: code, RedirectUri: oauthSetting.ConnectRedirectUri };

    $('#Loading').show();
    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        headers: {
            'Authorization': `Bearer ${token}`
        },
        data: JSON.stringify(postData),
        dataType: "json",
        complete: function (result) {
            bootbox.alert(result.responseJSON.Msg, function () {
                window.location.href = '../Profile';
            });
        }

    });
}

ResetOAuthSetting = function () {
    $('[id$=ConnectBtn]').show();
    $('[id$=DisconnectBtn]').hide();

    $('footer[id$=Status]').text('尚未綁定');
}

var Save = function () {
    let putUrl = `${settings.BaseDomainApiUrl}/User/Update`;
    $('#Loading').show();

    let data = {
        Name: $('#Name').val(),
        Email: $('#Email').val(),
        Account: $('#Account').val(),
        Password: $('#Password').val(),
        Description: $('#Description').val()
    };

    $.ajax({
        method: "PUT",
        url: putUrl,
        contentType: "application/json",
        headers: {
            'Authorization': `Bearer ${token}`
        },
        data: JSON.stringify(data),
        dataType: "text",
        success: function () {
            bootbox.alert("編輯成功");
        },
        error: function (result) {
            bootbox.alert({
                message: `編輯失敗:${JSON.parse(result.responseText).Msg}`
            });
        },
        complete: function (result) {
            $('#Loading').hide();
        }

    });
}
