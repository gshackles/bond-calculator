FROM microsoft/windowsservercore
COPY bin/Release/ /

ADD http://chromedriver.storage.googleapis.com/2.35/chromedriver_win32.zip /
ADD https://dl.google.com/tag/s/dl/chrome/install/googlechromestandaloneenterprise64.msi /

RUN msiexec /i googlechromestandaloneenterprise64.msi /quiet

ENTRYPOINT BondCalculator.exe