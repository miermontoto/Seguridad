New-SelfSignedCertificate -Type Custom `
-Subject "CN=zpser.as" -DnsName "zpser.as", "www.zpser.es", "www.zpser.com" `
-KeyAlgorithm RSA -KeyLength 2048 -KeySpec Signature -KeyExportPolicy Exportable `
-KeyUsageProperty All -KeyUsage None `
-HashAlgorithm sha256 `
-Signer $cert `
-CertStoreLocation "Cert:\CurrentUser\My"