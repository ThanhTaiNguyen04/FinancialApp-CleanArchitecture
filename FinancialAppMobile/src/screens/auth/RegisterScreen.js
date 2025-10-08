import React, { useState } from 'react';
import { 
  View, 
  Text, 
  StyleSheet, 
  StatusBar, 
  TouchableOpacity, 
  KeyboardAvoidingView,
  ScrollView,
  Platform,
  Alert
} from 'react-native';
import GradientBackground from '../../components/GradientBackground';
import CustomInput from '../../components/CustomInput';
import CustomButton from '../../components/CustomButton';
import { Colors } from '../../utils/colors';
import { SCREEN_NAMES } from '../../utils/constants';
import ApiService from '../../services/apiService';

const RegisterScreen = ({ navigation }) => {
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    phone: '',
    password: '',
    confirmPassword: '',
  });
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);

  const handleInputChange = (field, value) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => ({
        ...prev,
        [field]: ''
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};

    if (!formData.fullName.trim()) {
      newErrors.fullName = 'Họ tên không được để trống';
    } else if (formData.fullName.trim().length < 2) {
      newErrors.fullName = 'Họ tên phải có ít nhất 2 ký tự';
    }

    if (!formData.email.trim()) {
      newErrors.email = 'Email không được để trống';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Email không đúng định dạng';
    }

    if (formData.phone && formData.phone.length > 0) {
      if (!/^[0-9+\-\s()]+$/.test(formData.phone)) {
        newErrors.phone = 'Số điện thoại không đúng định dạng';
      }
    }

    if (!formData.password.trim()) {
      newErrors.password = 'Mật khẩu không được để trống';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Mật khẩu phải có ít nhất 6 ký tự';
    }

    if (!formData.confirmPassword.trim()) {
      newErrors.confirmPassword = 'Xác nhận mật khẩu không được để trống';
    } else if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Mật khẩu xác nhận không khớp';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleRegister = async () => {
    if (!validateForm()) return;

    setLoading(true);
    try {
      const registerData = {
        fullName: formData.fullName.trim(),
        email: formData.email.trim().toLowerCase(),
        phone: formData.phone.trim(),
        password: formData.password,
        confirmPassword: formData.confirmPassword,
      };

      const response = await ApiService.register(registerData);
      
      if (response.success) {
        Alert.alert(
          'Thành công', 
          'Đăng ký tài khoản thành công!', 
          [
            {
              text: 'OK',
              onPress: () => navigation.replace(SCREEN_NAMES.MAIN)
            }
          ]
        );
      } else {
        Alert.alert('Lỗi', response.message || 'Đăng ký thất bại');
      }
    } catch (error) {
      console.error('Register error:', error);
      Alert.alert(
        'Lỗi', 
        error.response?.data?.message || 'Có lỗi xảy ra, vui lòng thử lại'
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <GradientBackground style={styles.container}>
      <StatusBar barStyle="light-content" backgroundColor={Colors.gradientStart} />
      
      <KeyboardAvoidingView 
        style={styles.flex}
        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
      >
        <ScrollView 
          contentContainerStyle={styles.scrollContainer}
          keyboardShouldPersistTaps="handled"
          showsVerticalScrollIndicator={false}
        >
          {/* Header */}
          <View style={styles.header}>
            <Text style={styles.title}>Tạo Tài Khoản</Text>
            <Text style={styles.subtitle}>Đăng ký để bắt đầu quản lý tài chính</Text>
          </View>

          {/* Form Card */}
          <View style={styles.formCard}>
            <CustomInput
              label="Full Name"
              value={formData.fullName}
              onChangeText={(text) => handleInputChange('fullName', text)}
              placeholder="Nguyen Van A"
              autoCapitalize="words"
              error={errors.fullName}
            />

            <CustomInput
              label="Email"
              value={formData.email}
              onChangeText={(text) => handleInputChange('email', text)}
              placeholder="example@example.com"
              keyboardType="email-address"
              autoCapitalize="none"
              error={errors.email}
            />

            <CustomInput
              label="Số Điện Thoại"
              value={formData.phone}
              onChangeText={(text) => handleInputChange('phone', text)}
              placeholder="+123 456 789"
              keyboardType="phone-pad"
              error={errors.phone}
            />

            <CustomInput
              label="Mật Khẩu"
              value={formData.password}
              onChangeText={(text) => handleInputChange('password', text)}
              placeholder="••••••••"
              secureTextEntry
              error={errors.password}
            />

            <CustomInput
              label="Xác Nhận Mật Khẩu"
              value={formData.confirmPassword}
              onChangeText={(text) => handleInputChange('confirmPassword', text)}
              placeholder="••••••••"
              secureTextEntry
              error={errors.confirmPassword}
            />

            {/* Terms and Conditions */}
            <View style={styles.termsContainer}>
              <Text style={styles.termsText}>
                Bằng cách tiếp tục, bạn đồng ý với{' '}
                <Text style={styles.termsLink}>Điều khoản sử dụng</Text>
                {' '}và{' '}
                <Text style={styles.termsLink}>Chính sách bảo mật</Text>.
              </Text>
            </View>

            <CustomButton
              title="Đăng Ký"
              onPress={handleRegister}
              loading={loading}
              style={styles.registerButton}
            />
          </View>

          {/* Login Link */}
          <View style={styles.loginContainer}>
            <TouchableOpacity 
              style={styles.loginLink}
              onPress={() => navigation.navigate(SCREEN_NAMES.LOGIN)}
            >
              <Text style={styles.loginLinkText}>
                Không có tài khoản? <Text style={styles.loginLinkHighlight}>Đăng ký</Text>
              </Text>
            </TouchableOpacity>
          </View>
        </ScrollView>
      </KeyboardAvoidingView>
    </GradientBackground>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  flex: {
    flex: 1,
  },
  scrollContainer: {
    flexGrow: 1,
    paddingHorizontal: 20,
    paddingTop: 60,
    paddingBottom: 30,
  },
  header: {
    alignItems: 'center',
    marginBottom: 30,
  },
  title: {
    fontSize: 28,
    fontWeight: 'bold',
    color: Colors.white,
    marginBottom: 8,
  },
  subtitle: {
    fontSize: 14,
    color: 'rgba(255, 255, 255, 0.8)',
    textAlign: 'center',
  },
  formCard: {
    backgroundColor: 'rgba(255, 255, 255, 0.95)',
    borderRadius: 20,
    padding: 25,
    marginBottom: 20,
    elevation: 10,
    shadowColor: Colors.shadowColor,
    shadowOffset: { width: 0, height: 5 },
    shadowOpacity: 0.2,
    shadowRadius: 10,
  },
  termsContainer: {
    marginBottom: 20,
    paddingHorizontal: 5,
  },
  termsText: {
    fontSize: 12,
    color: Colors.textSecondary,
    textAlign: 'center',
    lineHeight: 18,
  },
  termsLink: {
    color: Colors.primary,
    textDecorationLine: 'underline',
  },
  registerButton: {
    marginTop: 5,
  },
  loginContainer: {
    alignItems: 'center',
  },
  loginLink: {
    paddingVertical: 15,
  },
  loginLinkText: {
    color: 'rgba(255, 255, 255, 0.8)',
    fontSize: 14,
  },
  loginLinkHighlight: {
    color: Colors.white,
    fontWeight: 'bold',
    textDecorationLine: 'underline',
  },
});

export default RegisterScreen;