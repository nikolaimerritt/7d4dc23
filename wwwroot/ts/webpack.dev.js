const path = require("path");
const { VueLoaderPlugin } = require("vue-loader");

module.exports = {
    entry: {
        gameRoot: {
            import: "./src/game-root/main.ts",
        },
        leaderboard: {
            import: "./src/leaderboard/main.ts",
        },
        history: {
            import: "./src/history/main.ts",
        },
    },
    module: {
        rules: [
            {
                test: /\.css$/,
                use: ["style-loader", "css-loader"],
            },
            {
                test: /\.scss/,
                use: ["style-loader", "css-loader", "sass-loader"],
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
                loader: "vue-loader",
                options: {
                    loaders: {
                                   'scss': 'vue-style-loader!css-loader!sass-loader',
            'sass': 'vue-style-loader!css-loader!sass-loader?indentedSyntax'
                    }
                }
            },
            {
                test: /\.(png|jpg|jpe?g|gif|mp3)$/i,
                loader: "file-loader",
            },
            {
                test: /\.svg/,
                use: [
                    "babel-loader",
                    {
                        loader: "vue-svg-loader",
                        options: {
                            svgo: {
                            plugins: [
                                {removeDoctype: true},
                                {removeComments: true},
                                {cleanupIDs: false},
                                {collapseGroups: false},
                                {removeEmptyContainers: false}
                            ]
                            }
                        }
                    }
                ]
            }
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
