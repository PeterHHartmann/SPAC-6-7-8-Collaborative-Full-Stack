# SPAC - Week 8 - Inventory-Management
This project is a solo continuation of the application developed in collaboration with __Thyge123__ and __EmilSalomonsen__ at SPAC, week 6-7.  
The focus of this project is on containerization and continuous integration of a previously developed application.

## Disclaimer
This project is in no way meant to be production ready or deployed. It is merely an exercise in dockerization and CI.

## Contents
This repository functions as a mono-repo containing multiple elements of the application whole. Those elements include:

- __Backend: C# REST API__  
  *Using __ASP.NET Core__ and __Entity Framework Core__*  
	Used to serve product inventory data from the database

- __Frontend: React SPA__  
  *Using __React, TypeScript__ and __Vite__*  
	Serving an administrator CMS GUI for managing inventory

- __Database: PostgreSQL__  
	Used for persisting inventory data

- __pgAdmin GUI__  
	Used for inspecting and interacting with the PostgreSQL database during development

- __Nginx Server__  
	*Used as a reverse proxy forwarding HTTP/HTTPS traffic for both the backend and frontend*

## Development Dependencies
- [Docker](https://www.docker.com/get-started/)
- [.NET SDK 8.0 & ASP.NET Core Runtime 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Node.js v22.15.0 or higher](https://nodejs.org/en/download)

## Get Started
1. __Set up environment variables:__  
	The docker configuration requires an environment variable file named *'.env'* to exist in the project root.  
	In the project a file named *'.env.example'* is included as a reference to what variables the real file must contain. You can simply create a copy of this file with the name *'.env'* or modify the variables however you want.  

2. __Spin up the application:__  
	You should now be ready to spin up the whole application with the following command:
	```sh
	COMPOSE_BAKE=true docker compose build
	COMPOSE_BAKE=true docker compose up
	```
	or use the included Makefile:
	```
	make up
	```

3. __View the frontend:__  
	Your should not be able to view the frontend on [https://localhost/](https://localhost/)  
	The site will require you to log in. You can do this with the example admin user:  
			- Username: "admin"  
			- Password: "password"  
			
4. __You can also find the API root on:__  
	[https://localhost/api/](https://localhost/api/)