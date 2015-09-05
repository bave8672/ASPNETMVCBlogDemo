// Touch handler converts touches to clicks http://stackoverflow.com/a/6141093
function touchHandler(event) {
    var touches = event.changedTouches,
        first = touches[0],
        type = "";

    switch (event.type) {
        case "touchstart": type = "mousedown"; break;
        case "touchmove": type = "mousemove"; break;
        case "touchend": type = "mouseup"; break;
        default: return;
    }

    var simulatedEvent = document.createEvent("MouseEvent");
    simulatedEvent.initMouseEvent(type, true, true, window, 1,
                              first.screenX, first.screenY,
                              first.clientX, first.clientY, false,
                              false, false, false, 0/*left*/, null);

    first.target.dispatchEvent(simulatedEvent);
    event.preventDefault();
}

// Call this after the dom has loaded if you plan on handling touches
function touchInit() {
    var touchEls = document.querySelectorAll('.touch, button, a');
    for (var el in touchEls)
    {
        el.removeEventListener("touchstart", touchHandler, true);
        el.removeEventListener("touchmove", touchHandler, true);
        el.removeEventListener("touchend", touchHandler, true);
        el.removeEventListener("touchcancel", touchHandler, true);
        el.addEventListener("touchstart", touchHandler, true);
        el.addEventListener("touchmove", touchHandler, true);
        el.addEventListener("touchend", touchHandler, true);
        el.addEventListener("touchcancel", touchHandler, true);
    }
}

touchInit();