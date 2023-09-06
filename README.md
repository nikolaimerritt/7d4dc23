## Docker

The easiest way to run this is to use docker. Start by creating the container:

    docker build -t whudunnit -f Dockerfile .

Then run the container:

    docker run -p 80:80 --restart always --name whudunnit-app whudunnit

You can access the app at http://localhost:80.