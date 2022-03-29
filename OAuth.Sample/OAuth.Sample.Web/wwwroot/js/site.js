var settings = {
    BaseDomainApiUrl: '',
    OAuthSettings: null
}; 

var site = function() {
    

    return {
        init: function (obj) {
            Object.assign(settings, obj);
            console.log('settings', settings);
        }
    };
}
