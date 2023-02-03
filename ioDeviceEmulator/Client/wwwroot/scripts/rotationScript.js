

var addedListener = false;
export function setImageRotation(element, degrees, dotNetObject) {
    // element.style.transition = "transform 2s ease-out";
    element.style.transition = "";

    element.style.transition = "transform 2s cubic-bezier(0.45, 0.05, 0.55, 0.95)"

    element.style.transform = "rotate(" + degrees + "deg)";

    if (!addedListener) {
        element.addEventListener('transitionend', () => {
            logToConsoleWhenTransitionFinished(dotNetObject)
        });
        addedListener = true;
    }
}


function logToConsoleWhenTransitionFinished(dotNetObject) {
    dotNetObject.invokeMethodAsync('ioDeviceEmulator.Client', 'InformationWhenTransitionFinished');
}

