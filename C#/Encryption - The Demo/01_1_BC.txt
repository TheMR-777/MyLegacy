public static RSAParameters ImportPemKey_BC(string keyPath)
{
    using var reader = new StreamReader(keyPath);
    return new PemReader(reader).ReadObject() switch
    {
        RsaPrivateCrtKeyParameters privateKey => DotNetUtilities.ToRSAParameters(privateKey),
        RsaKeyParameters publicKey => DotNetUtilities.ToRSAParameters(publicKey),
        _ => new RSAParameters()
    };
}
