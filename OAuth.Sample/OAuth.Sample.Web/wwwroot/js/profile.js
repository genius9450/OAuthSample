window.onload = function () {
    let url = new URL(this.location.href);
    if (url.searchParams.has('userId') ){
        UserLogin(url.searchParams.get("userId"));
    }
    else if (url.searchParams.has('code') && url.searchParams.has('state')) {
        let stateArr = url.searchParams.get('state').split('_');
        switch (stateArr[0]) {
            case 'LineNotify':
                SubscribeLineNotify(stateArr[1], url.searchParams.get('code'));
            break;
        }

    }

}

var UserLogin = function(userId) {
    let getUrl = `${$("#BaseDomainApiUrl").val()}/User/${userId}`;
    $.ajax({
        method: "GET",
        url: getUrl,
        dataType: "json",
        success: function (result) {
            console.log('success', result);

            $('#Name').text(result.Name);
        },
        error: function (result) {
            console.log('error', result);
        }
    });
}

var UserLineNotifyAuth = function () {
    let url = new URL(this.location.href);
    let state = `LineNotify_${url.searchParams.get("userId")}`;

    let authUrl = 'https://notify-bot.line.me/oauth/authorize?';
    authUrl += 'response_type=code';
    authUrl += `&client_id=JXHlpy3ASuitjhpFEYeymW`;
    authUrl += `&redirect_uri=https://localhost:44350/Profile`;
    authUrl += `&state=${state}`;
    authUrl += '&scope=notify';
    window.location.href = authUrl; 
}

var SubscribeLineNotify = function(userId, code) {
    let postUrl = `${$("#BaseDomainApiUrl").val()}/LineNotify/Subscribe`;
    let postData = { UserId: userId, Code: code };

    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        data: JSON.stringify(postData),
        dataType: "json",
        success: function (result) {
            console.log('success', result);
        },
        error: function (result) {
            console.log('error', result);
        }
    });
}

var UnSubscribeLineNotify = function () {
    let url = new URL(this.location.href);
    let postUrl = `${$("#BaseDomainApiUrl").val()}/LineNotify/UnSubscribe`;
    let postData = { UserId: url.searchParams.get("userId") };

    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        data: JSON.stringify(postData),
        dataType: "json",
        success: function (result) {
            console.log('success', result);
        },
        error: function (result) {
            console.log('error', result);
        }
    });
}

var SendMessage = function() {
    let postUrl = `${$("#BaseDomainApiUrl").val()}/LineNotify/Notify`;
    let postData = { Message: 'Hello World!' };

    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        data: JSON.stringify(postData),
        dataType: "json",
        success: function (result) {
            console.log('success', result);
        },
        error: function (result) {
            console.log('error', result);
        }
    });
}