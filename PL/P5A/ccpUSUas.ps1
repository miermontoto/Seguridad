New-SelfSignedCertificate -Type Custom `
-Subject "CN=zpusu.as" -DnsName "zpusu.as" `
-KeyAlgorithm RSA -KeyLength 2048 -KeySpec Signature -KeyExportPolicy Exportable `
-KeyUsageProperty All -KeyUsage None `
-HashAlgorithm sha256 -Signer $cert `
-CertStoreLocation "Cert:\CurrentUser\My"