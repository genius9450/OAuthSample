var token;

window.onload = function () {
    token = Cookies.get("JwtToken");
    let url = new URL(this.location.href);

    if (url.searchParams.has('code') && url.searchParams.has('state')) {
        let state = url.searchParams.get('state');
        if (state != Cookies.get("LineNotifyState", { path: this.location.pathname })) {
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
        }
    }

    GetUserData();
}

var GetUserData = function () {
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
            $('#Name').text(result.Name);
            $('#Description').text(result.Description);
            $('#Email').text(result.Email);

            handleFunctionBtn(result);

            $('#Loading').hide();
        },
        error: function (result) {
            if (result.status === 401) window.location.href = "/";
        }
    });
}

var handleFunctionBtn = function (result) {
    let hasLineNotify = result.UserOAuthData.findIndex(x => x.ProviderType == 'LineNotify');
    if (hasLineNotify > -1) {
        // 已訂閱
        $('#SubscribeLineNotify').hide();
        $('#SendMessage').show();
        $('#UnSubscribeLineNotify').show();
    }
    else {
        // 未訂閱
        $('#SubscribeLineNotify').show();
        $('#SendMessage').hide();
        $('#UnSubscribeLineNotify').hide();
    }
}

var UserLineNotifyAuth = function () {
    let state = `LineNotify_${Date.now()}`;
    Cookies.set("LineNotifyState", state, { path: this.location.pathname });

    let oauthSetting = settings.OAuthSettings.find(x => x.ProviderType == 'LineNotify');
    let authUrl = 'https://notify-bot.line.me/oauth/authorize?';
    authUrl += 'response_type=code';
    authUrl += `&client_id=${oauthSetting.ClientId}`;
    authUrl += `&redirect_uri=${oauthSetting.RedirectUri}`;
    authUrl += `&state=${state}`;
    authUrl += '&scope=notify';
    window.location.href = authUrl;
}

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
            bootbox.alert({
                message: "Line Notify取消訂閱成功"
            });

            GetUserData();
        },
        error: function () {
            bootbox.alert({
                message: "Line Notify取消訂閱失敗"
            });
        }
    });
}

var SendMessageConfirm = function () {
    bootbox.prompt({
        title: '發送訊息',
        value: 'Hello World!',
        callback: function (message) {
            if (message) {
                $('.bootbox-input-text').removeClass('is-invalid');
                SendMessage(message);
            } else if (message === '') {
                $('.bootbox-input-text').addClass('is-invalid');
                return false;
            }
        }
    });
}

var SendMessage = function (message) {

    let postUrl = `${settings.BaseDomainApiUrl}/LineNotify/Notify`;
    let postData = { Message: message };

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
