import React from 'react';
import { View, Text, StyleSheet, StatusBar, TouchableOpacity } from 'react-native';
import GradientBackground from '../../components/GradientBackground';
import CompassLogo from '../../components/CompassLogo';
import CustomButton from '../../components/CustomButton';
import { Colors } from '../../utils/colors';
import { SCREEN_NAMES } from '../../utils/constants';

const WelcomeScreen = ({ navigation }) => {
  return (
    <GradientBackground style={styles.container}>
      <StatusBar barStyle="light-content" backgroundColor={Colors.gradientStart} />
      
      <View style={styles.content}>
        {/* Logo */}
        <View style={styles.logoContainer}>
          <CompassLogo size={120} />
        </View>
        
        {/* Welcome Text */}
        <View style={styles.textContainer}>
          <Text style={styles.welcomeText}>Welcome to</Text>
          <Text style={styles.appName}>DeepSynTech</Text>
        </View>
        
        {/* Action Buttons */}
        <View style={styles.buttonContainer}>
          <CustomButton
            title="Đăng Nhập"
            onPress={() => navigation.navigate(SCREEN_NAMES.LOGIN)}
            variant="primary"
            style={styles.button}
          />
          
          <CustomButton
            title="Đăng Ký"
            onPress={() => navigation.navigate(SCREEN_NAMES.REGISTER)}
            variant="secondary"
            style={styles.button}
          />
        </View>
        
        {/* Forgot Password Link */}
        <TouchableOpacity style={styles.forgotPasswordContainer}>
          <Text style={styles.forgotPasswordText}>Quên mật khẩu?</Text>
        </TouchableOpacity>
      </View>
    </GradientBackground>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  content: {
    flex: 1,
    paddingHorizontal: 40,
    justifyContent: 'center',
    alignItems: 'center',
  },
  logoContainer: {
    marginBottom: 60,
  },
  textContainer: {
    alignItems: 'center',
    marginBottom: 80,
  },
  welcomeText: {
    fontSize: 32,
    color: Colors.white,
    fontWeight: '300',
    marginBottom: 8,
  },
  appName: {
    fontSize: 32,
    color: Colors.white,
    fontWeight: 'bold',
    letterSpacing: 1,
  },
  buttonContainer: {
    width: '100%',
    marginBottom: 30,
  },
  button: {
    marginBottom: 16,
  },
  forgotPasswordContainer: {
    paddingVertical: 8,
  },
  forgotPasswordText: {
    color: Colors.white,
    fontSize: 14,
    textDecorationLine: 'underline',
  },
});

export default WelcomeScreen;