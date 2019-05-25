# nas-crawler

## Introduction
A crawler for online NAS reachable from Google search engine.

## Installation

Install MySQL server and create a schema named "nascan".

Clone the project:
```
git clone https://github.com/tassoneroberto/nas-crawler.git
cd crawler

```
Open the project in Visual Studio and run it.

It will search the web for public NAS and it will store found files in the MySQL database.

The folder "web" contains a very simple web interface to query the collected data.

Edit the file "web/connect.php" and insert your database credentials.

You still need a web server in order to run the web interface. You can deploy it in a remote host or in local.
