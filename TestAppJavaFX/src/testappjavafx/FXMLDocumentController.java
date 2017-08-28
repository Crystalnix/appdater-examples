/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package testappjavafx;

import java.io.IOException;
import java.net.URL;
import java.util.ResourceBundle;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;

/**
 *
 * @author mikhail
 */
public class FXMLDocumentController implements Initializable {
        
    @FXML
    private void handleCheckUpdatesAction(ActionEvent event) throws IOException {
        try
        {
            String[] cmdarray = {
                "C:\\Program Files (x86)\\Crystalnix\\Update\\1.3.99.0\\Update.exe",
                "/machine", 
                "/ua", 
                "/installsource", 
                "ondemand",
            };
            final Process proc = Runtime.getRuntime().exec(cmdarray);
            proc.waitFor();
        }
        catch(InterruptedException e)
        {
        }
    }
    
    @Override
    public void initialize(URL url, ResourceBundle rb) {
        // TODO
    }    
    
}
