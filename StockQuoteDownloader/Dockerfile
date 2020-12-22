#FROM microsoft/dotnet:2.1-sdk
#WORKDIR /app
#
#COPY *.csproj ./
#RUN dotnet restore
#
#COPY . ./
#RUN dotnet publish -c Release -o out
#ENTRYPOINT ["dotnet", "out/StockQuoteDownloader.dll"

FROM microsoft/dotnet:2.1-sdk

#COPY palo.cer ./
#RUN  Import-Certificate -filepath palo-alto-web-ca-2018.pem -certstorelocation cert:\currentUser\Root -confirm -whatif

COPY ./stocklist.txt temp/stocklist.txt

WORKDIR /app

COPY . ./

#RUN mkdir c:\temp
#COPY stocklist.txt c:\temp\

#RUN certoc -addstore root henterprise.cer 
#RUN certoc -addstore root palo.cer

COPY *.csproj ./

RUN dotnet restore 

#COPY . ./

RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "out/StockQuoteDownloader.dll"]
#CMD ["powershell"]
