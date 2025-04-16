/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Components/**/*.{razor,html}',
        '../PhotoSharingApplication.Client/**/*.{razor,html}',
    ],
    theme: {
        extend: {},
    },
    safelist: [
        {
            pattern: /.*/, //TODO this is not production safe!!!!!!!!!!!!!!!!!!
        }
    ],
    plugins: [],
}