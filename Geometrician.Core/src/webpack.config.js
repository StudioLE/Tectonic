const path = require('path');

module.exports = {
  entry: './Scripts/Bundle.ts',
  output: {
    filename: 'bundle.js',
    path: path.resolve(__dirname, 'wwwroot/dist'),
    library: {
      name: 'exports',
      type: 'umd',
    },
  },
  optimization: {
    usedExports: false,
  },
  devtool: false,
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: 'ts-loader',
        exclude: /node_modules/,
      },
    ],
  },
  resolve: {
    extensions: ['.tsx', '.ts', '.js'],
  },
};
