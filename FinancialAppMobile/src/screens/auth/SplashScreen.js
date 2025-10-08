import React, { useEffect } from 'react';
import { View, Text, StyleSheet, StatusBar } from 'react-native';
import GradientBackground from '../../components/GradientBackground';
import CompassLogo from '../../components/CompassLogo';
import { Colors } from '../../utils/colors';
import { SCREEN_NAMES } from '../../utils/constants';

const SplashScreen = ({ navigation }) => {
  useEffect(() => {
    const timer = setTimeout(() => {
      navigation.replace(SCREEN_NAMES.WELCOME);
    }, 3000);

    return () => clearTimeout(timer);
  }, [navigation]);

  return (
    <GradientBackground style={styles.container}>
      <StatusBar barStyle="light-content" backgroundColor={Colors.gradientStart} />
      
      <View style={styles.content}>
        <CompassLogo size={150} />
        <Text style={styles.title}>DeepSynTech</Text>
        <Text style={styles.subtitle}>Financial Management</Text>
      </View>
      
      <View style={styles.footer}>
        <Text style={styles.version}>Version 1.0.0</Text>
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
    justifyContent: 'center',
    alignItems: 'center',
    paddingHorizontal: 40,
  },
  title: {
    fontSize: 36,
    fontWeight: '300',
    color: Colors.white,
    letterSpacing: 2,
    marginTop: 40,
    marginBottom: 8,
  },
  subtitle: {
    fontSize: 16,
    color: 'rgba(255, 255, 255, 0.8)',
    fontWeight: '300',
  },
  footer: {
    paddingBottom: 40,
    alignItems: 'center',
  },
  version: {
    fontSize: 12,
    color: 'rgba(255, 255, 255, 0.6)',
  },
});

export default SplashScreen;