import java.security.MessageDigest;
import java.math.BigInteger;

public class Main {
    public static void main(String[] args) {
        try {
            String input = "fdsfgswrgdxaglawofjwivnskanviaoq";
            MessageDigest md = MessageDigest.getInstance("SHA-256");
            byte[] messageDigest = md.digest(input.getBytes("UTF-8"));

            BigInteger bigint = new BigInteger(1, messageDigest);
            String hexText = bigint.toString(16);
            while (hexText.length() < 32) {
                hexText = "0".concat(hexText);
            }

            System.out.println(hexText.length() + " - " + hexText);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
