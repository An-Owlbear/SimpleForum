function openDropdown(object) {
    object.classList.add('dropdown-clicked');
    object.removeEventListener('click', object.openDropdown);
    object.addEventListener('click', object.closeDropdown);

    let dropdownItems = object.getElementsByClassName('dropdown-items');
    Array.from(dropdownItems).forEach(function (element) {
        element.classList.add('dropdown-items-visible');
    })
}

function closeDropdown(object) {
    object.classList.remove('dropdown-clicked');
    object.removeEventListener('click', object.closeDropdown);
    object.addEventListener('click', object.openDropdown);

    let dropdownItems = object.getElementsByClassName('dropdown-items');
    Array.from(dropdownItems).forEach(function (element) {
        element.classList.remove('dropdown-items-visible');
    })
}

let dropdowns = document.getElementsByClassName('dropdown');

Array.from(dropdowns).forEach(function (element) {
    element.openDropdown = () => openDropdown(element);
    element.closeDropdown = () => closeDropdown(element);
    element.addEventListener('click', element.openDropdown);
})

// Crops the image if it is not square
function cropImage(image, outputImage) {
    // Gets the canvas and calculates values
    let canvas = document.getElementById('canvas');
    let ctx = canvas.getContext('2d');
    let diff = Math.abs(image.width - image.height);
    let crop = Math.ceil(diff / 2);
    
    // If the width is greater crop the image horizontally, if height is greater crop vertically
    if (image.width > image.height) {
        canvas.height = image.height;
        canvas.width = image.height;
        ctx.drawImage(image, crop, 0, (image.width - diff), image.height, 0, 0, image.height, image.height);
    } else {
        canvas.height = image.width;
        canvas.width = image.width;
        ctx.drawImage(image, 0, crop, image.width, (image.height - diff), 0, 0, image.width, image.width);
    }
    outputImage.src = canvas.toDataURL();
}

// Shows a preview of the new profile picture after chosen
function updateProfileEdit() {
    let profileImg = document.getElementById('profilepicture-upload').files[0];
    let profileElement = document.getElementById('profilepicture-preview');
    let tempImage = document.getElementById('tempImage');
    let reader = new FileReader();
    reader.onload = function(e) {
        tempImage.src = e.target.result;
        tempImage.onload = () => cropImage(tempImage, profileElement);
    }
    reader.readAsDataURL(profileImg);
}

// Opens and closes the navigation bar
function toggleMenu(element, nav) {
    if (element.classList.contains("open")){
        nav.scrollTo(0,0);
        element.classList.remove("open");
    }
    else {
        element.classList.add("open");
    }
}

// Adds event listener to navigation bar
let navbar = document.querySelector('.navbar');
let navbarNav = document.querySelector('.navbar-nav');
let button = document.querySelector('.navbar .toggle-menu')
button.addEventListener('click', () => toggleMenu(navbar, navbarNav));