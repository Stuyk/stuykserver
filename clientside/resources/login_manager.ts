var loggedIn = false;
function setLoggedIn(value: boolean) { loggedIn = value };
API.onUpdate.connect(function () {
    if (loggedIn) {
        return;
    }
    if (API.getActiveCamera() !== null) {
        var camera = API.getActiveCamera();
        var rot = API.getCameraRotation(camera);
        API.setCameraRotation(camera, rot.Add(new Vector3(0, 0, 0.02)));
    }
});