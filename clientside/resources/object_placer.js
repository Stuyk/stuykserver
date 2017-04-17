var enabled = false;
var target = null;
API.onUpdate.connect(function () {
    if (enabled === false && target === null) {
        return;
    }
    API.disableControlThisFrame(24 /* Attack */);
    API.disableControlThisFrame(238 /* CursorCancel */);
    API.disableControlThisFrame(241 /* CursorScrollUp */);
    var aimPos = API.getPlayerAimCoords(API.getLocalPlayer());
    // Place Object
    if (API.isDisabledControlJustPressed(24 /* Attack */)) {
        API.triggerServerEvent("Object_Placement", API.getEntityPosition(target), API.getEntityRotation(target));
        enabled = false;
        target = null;
        return;
    }
    // Move Object
    API.setEntityPosition(target, aimPos.Add(new Vector3(0, 0, 1.3)));
    // Rotate Object
    if (API.isDisabledControlJustPressed(242 /* CursorScrollDown */)) {
        var rotation = API.getEntityRotation(target);
        API.setEntityRotation(target, rotation.Add(new Vector3(0, 0, -5)));
    }
    if (API.isDisabledControlJustPressed(241 /* CursorScrollUp */)) {
        var rotation = API.getEntityRotation(target);
        API.setEntityRotation(target, rotation.Add(new Vector3(0, 0, 5)));
    }
});
// Push object from server to client.
function attachObject(objectNet) {
    if (!API.doesEntityExist(objectNet)) {
        return;
    }
    target = objectNet;
}
