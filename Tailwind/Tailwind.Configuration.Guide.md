# Tailwind with ASP.NET Core MVC

The configuration of Tailwind with ASP.NET Core requires some work, and hopefully after finding some plugins, extensions, and scripts, this can be simplified further. This documentation will contain the most straight forward approach for the configuration.

## Step 01 - Setting up ASP.NET Core MVC Project
Use the Wizard provided by Visual Studio, or use the following command for .NET CLI:

    dotnet new mvc "Project name here"

## Step 02 - Initiating Node.JS for the current Project
This step is necessary for installing, and configuring the Tailwind, and installing additional package for final css cleaning for production.

Use the following command for initiating NPM, Tailwind, with PostCSS.

    npm init -y; npm install -D tailwindcss; npm install --save-dev @fullhuman/postcss-purgecss

This will also generate a `packages.json` file. You will need to add the following build script to your package.json file. This script will generate the Tailwind CSS output at `/wwwroot/css/styles.css`

    "scripts": {
        "css:build": "npx tailwindcss -i ./wwwroot/css/site.css -o ./wwwroot/css/styles.css --minify"
    }

## Step 03 - Creating config files for Tailwind and PostCSS
Create a new file named `postcss.config.js` with the following content:

    const purgecss = require('@fullhuman/postcss-purgecss')({
        content: ['./*/.cshtml'],
        defaultExtractor: content => content.match(/[\w-/:]+(?<!:)/g) || []
    })
    module.exports = {
        plugins: [
            require('tailwindcss'),
            require('autoprefixer'),
            ...process.env.NODE_ENV === 'production' ? [purgecss] : []
        ]
    }

Create a file `tailwind.config.js` file with:

    /** @type {import('tailwindcss').Config} */
    module.exports = {
        prefix: 'tw-',
        content: [
            // ASP.NET Core
            './Pages/**/*.cshtml', './Views/**/*.cshtml'

            // Ordinary
            //'./src/**/*.{html,ts,js,jsx,tsx}', '*.{html,js}'
        ],
        theme: {
           colors: {
           'primary': '#3A2E39',
           'secondary': '#F2EFEA'
           },
           extend: {},
        },
        plugins: [],
    }

Edit the ↑ file accordingly, it's just as I like it.

## Step 04 - Configuring the Tailwind now
Run the following command to initiate Tailwind for the current project

    npx tailwindcss init

Create site.css File: Under your `/wwwroot/css` folder, create a site.css file with the following imports:

    @tailwind base;
    @tailwind components;
    @tailwind utilities;

Edit the web .csproj file: Add the following to ensure that Tailwind CSS builds during the build/deployment phase.

    <ItemGroup>
        <UpToDateCheckBuilt Include="wwwroot/css/site.css" Set="Css" />
        <UpToDateCheckBuilt Include="tailwind.config.js" Set="Css" />
    </ItemGroup>
        <Target Name="Tailwind" BeforeTargets="Build">
        <Exec Command="npm run css:build"/>
    </Target>

Include CSS in the `_Layout.cshtml`: The path to the CSS file should be included in the layout file like so:

    <link rel="stylesheet" href="~/css/styles.css" asp-append-version="true" />

Finally, Remove the link tag from `_Layout.cshtml` which contains `site.css`

Note: The proper IntelliSense will work with VS Code only. For designing, use VS Code, but for any other development, use Visual Studio.