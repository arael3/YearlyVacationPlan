
const googleButton = document.getElementById('google-button');

googleButton.addEventListener('click', function (event) {
    ifWebview();
});

function ifWebview() {
    if (window.navigator.standalone || (window.navigator.userAgent.match(/(FBAN|FBAV)/) !== null)) {
        event.preventDefault();
        toastr.error("Możliwość zalogowania/zarejestrować się przy użyciu konta Google wymaga użycia standardowej przeglądarki (np. Chrome, Firefox, Opera)");
    }
}