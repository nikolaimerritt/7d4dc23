const path = require("path");
const { VueLoaderPlugin } = require("vue-loader");

module.exports = {
    entry: {
        main: {
            import: "./src/main.ts",
        },
    },
    module: {
        rules: [
            {
                test: /\.s?css$/,
                use: ["style-loader", "css-loader"],
            },
            {
                test: /\.tsx?$/,
                loader: "ts-loader",
                exclude: /node_modules/,
                options: {
                    appendTsSuffixTo: [/\.vue$/],
                },
            },
            {
                test: /\.vue$/,
                use: "vue-loader",
            },
        ],
    },
    resolve: {
        extensions: [".tsx", ".ts", ".js", ".vue"],
        alias: {
            vue$: "vue/dist/vue.esm.js",
        },
    },
    output: {
        filename: "[name].bundle.js",
        path: path.resolve(__dirname, "dist"),
    },
    optimization: {
        runtimeChunk: "single",
    },
    plugins: [new VueLoaderPlugin()],
        mode: "development",
    devtool: "inline-source-map",
};