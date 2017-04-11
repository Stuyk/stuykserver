API.onUpdate.connect(function () {
    if (!API.getEntityInvincible(API.getLocalPlayer()) &&
        API.getLocalPlayerInvincible()) {
        API.triggerServerEvent("AnticheatInvincibilityCheck");
    }
});
